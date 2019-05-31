using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransitPackage;

namespace RawData
{
    public static class Raw
    {
        static Mutex onDBChange = new Mutex();

        #region Select
        public static List<KircheElem> GetData()
        {
            List<KircheElem> res = null;
            try
            {
                using (KircheModelContext db = new KircheModelContext())
                {

                    var mainQuery =
                       from pr in db.Projects.AsQueryable()
                       join dst in db.District.AsQueryable() on pr.district_id equals dst.id
                       join bd_type in db.Building_Type.AsQueryable() on pr.building_type_id equals bd_type.id into temp_bd_type
                       join sc in db.Scope.AsQueryable() on pr.scope_id equals sc.id into temp_sc
                       join imp in db.Implementer.AsQueryable() on pr.implementer_id equals imp.id into temp_imp
                       join st in db.State.AsQueryable() on pr.state_id equals st.id into temp_st
                       join ch in db.Church.AsQueryable() on pr.church_id equals ch.id into temp_ch
                       from ch in temp_ch.DefaultIfEmpty()
                       from bd_type in temp_bd_type.DefaultIfEmpty()
                       from sc in temp_sc.DefaultIfEmpty()
                       from imp in temp_imp.DefaultIfEmpty()
                       from st in temp_st.DefaultIfEmpty()
                       select new
                       {
                           Id = pr.id,
                           Church_District = dst.name,
                           Church = ch.name,
                           Year_Start = pr.year_start,
                           Year_End = pr.year_end,
                           Price = pr.price,
                           Description = pr.description,
                       };

                    var mtmQuery =
                        from pr in db.Projects
                        join dst in db.District on pr.district_id equals dst.id
                        join pr_pr_type in db.Projects_Project_Type on new { PrId = pr.id, DstId = pr.district_id }
                        equals new { PrId = pr_pr_type.project_id, DstId = pr_pr_type.district_id }
                        into pr_types
                        from type in pr_types
                        join pt in db.Project_Type on type.project_type_id equals pt.id
                        select new
                        {
                            Id = pr.id,
                            Church_District = dst.name,
                            Project_Type = pt.name
                        };


                    res = mainQuery.Select(x => new KircheElem
                    {
                        Id = x.Id,
                        Church = x.Church,
                        Church_District = x.Church_District,
                        Year_Start = x.Year_Start,
                        Year_End = x.Year_End,
                        Price = x.Price,
                        Description = x.Description,
                    }).ToList();

                    foreach (var elem in res)
                        foreach (var item in mtmQuery)
                            if (elem.Id == item.Id
                                && elem.Church_District == item.Church_District)
                            {
                                elem.Project_Type.Add(item.Project_Type);
                            }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return res;
        }

        public static CategoryCollections GetCategories()
        {
            CategoryCollections res = new CategoryCollections();

            try
            {
                using (KircheModelContext db = new KircheModelContext())
                {

                    var buildingQuery =
                       from bd_type in db.Building_Type.AsQueryable()
                       select new { bd_type.name };

                    foreach (var item in buildingQuery)
                        res.BuildingType.Add(item.name);


                    var projectTypeQuery =
                        from pr_type in db.Project_Type.AsQueryable()
                        select new { pr_type.name };

                    foreach (var item in projectTypeQuery)
                        res.ProjectType.Add(item.name);


                    var districtQuery =
                        from dst_type in db.District.AsQueryable()
                        select new { dst_type.name };

                    foreach (var item in districtQuery)
                        res.ChurchDistrict.Add(item.name);


                    var statetQuery =
                        from st_type in db.State.AsQueryable()
                        select new { st_type.name };

                    foreach (var item in statetQuery)
                        res.State.Add(item.name);


                    var scopeQuery =
                        from sc_type in db.Scope.AsQueryable()
                        select new { sc_type.name };

                    foreach (var item in scopeQuery)
                        res.Scope.Add(item.name);


                    var cityQuery =
                        from c_type in db.City.AsQueryable()
                        select new { c_type.name };

                    foreach (var item in cityQuery)
                        res.City.Add(item.name);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return res;
        }

        public static bool PasswordCheck(string district, string key)
        {
            try
            {
                using (KircheModelContext db = new KircheModelContext())
                {
                    var districtPassQuery =
                        from dst_type in db.District.AsQueryable()
                        where dst_type.name == district
                        select new { dst_type.pass };

                    return key == districtPassQuery.FirstOrDefault().pass;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        #endregion

        #region Update
        public static bool Update(List<KircheElem> elems)
        {
            onDBChange.WaitOne();

            try
            {
                using (KircheModelContext db = new KircheModelContext())
                {
                    foreach (var item in elems)
                    {
                        var projectExist =
                            from pr in db.Projects.AsQueryable()
                            join dis in db.District.AsQueryable() on pr.district_id equals dis.id
                            where pr.id == item.Id && dis.name == item.Church_District
                            select pr;

                        if (projectExist.Count() == 0)
                        {
                            Projects NewProject = new Projects();
                            SetNewProjectKey(item, NewProject, db);
                            UpdateProject(item, NewProject, db);
                            db.Projects.Add(NewProject);
                        }
                        else
                            UpdateProject(item, projectExist.First(), db);

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            onDBChange.ReleaseMutex();
            return true;
        }

        private static void SetNewProjectKey(KircheElem item, Projects newProject, KircheModelContext db)
        {
            newProject.id = item.Id;
            newProject.district_id = (from dis in db.District
                                      where dis.name == item.Church_District
                                      select dis.id).First();
        }

        private static void UpdateProject(KircheElem item, Projects updateProject, KircheModelContext db)
        {
            ChurchOperation(item, updateProject, db);
            ProjectTypeOperation(item, updateProject, db);
            OtherOperation(item, updateProject, db);
        }

        private static void OtherOperation(KircheElem item, Projects project, KircheModelContext db)
        {
            project.year_start = item.Year_Start;
            project.year_end = item.Year_End;
            project.price = item.Price;
            project.description = item.Description;
        }

        private static void ProjectTypeOperation(KircheElem item, Projects project, KircheModelContext db)
        {
            foreach (var prType in item.Project_Type)
            {
                if (!string.IsNullOrWhiteSpace(prType))
                {
                    var prTypeExist =
                        from prtp in db.Project_Type.AsQueryable()
                        where prtp.name == prType
                        select prtp;

                    if (prTypeExist.Count() == 0)
                    {
                        Project_Type newPrType = new Project_Type { name = prType };
                        db.Project_Type.Add(newPrType);
                        db.SaveChanges();
                    }
                }
            }

            //delete old Projects Type
            var oldProjects_Projects_Type = db.Projects_Project_Type
                .Where(elem => elem.project_id == item.Id
                    && elem.district_id == (from dis in db.District.AsQueryable()
                                            where dis.name == item.Church_District
                                            select dis.id).FirstOrDefault());

            db.Projects_Project_Type.RemoveRange(oldProjects_Projects_Type);

            //add new Projects Type
            List<Projects_Project_Type> newProjects_Projects_Type =
                new List<Projects_Project_Type>
                (
                    item.Project_Type.Select(
                        projectType => new Projects_Project_Type()
                        {
                            project_id = item.Id,

                            district_id = (from dis in db.District.AsQueryable()
                                           where dis.name == item.Church_District
                                           select dis.id).FirstOrDefault(),

                            project_type_id = (from prtp in db.Project_Type.AsQueryable()
                                               where prtp.name == projectType
                                               select prtp.id).FirstOrDefault()
                        })
                    );

            db.Projects_Project_Type.AddRange(newProjects_Projects_Type);
        }

        private static void ChurchOperation(KircheElem item, Projects project, KircheModelContext db)
        {
            if (!string.IsNullOrWhiteSpace(item.Church))
            {
                var itemChurchExist =
                    from ch in db.Church.AsQueryable()
                    where ch.name == item.Church
                    select ch;

                if (itemChurchExist.Count() == 0)
                {
                    Church newChurch = new Church { name = item.Church };
                    db.Church.Add(newChurch);
                    db.SaveChanges();
                }

                project.church_id =
                    (from ch in db.Church.AsQueryable()
                     where ch.name == item.Church
                     select ch.id).First();
            }
            else
                project.church_id = null;
        }

        #endregion

        #region Delete
        public static bool Delete(Dictionary<int, string> forDelete)
        {
            onDBChange.WaitOne();

            try
            {
                using (KircheModelContext db = new KircheModelContext())
                {
                    foreach (var item in forDelete)
                    {
                        var projectExist =
                            from pr in db.Projects.AsQueryable()
                            join dis in db.District.AsQueryable() on pr.district_id equals dis.id
                            where pr.id == item.Key && dis.name == item.Value
                            select pr;

                        if (projectExist.Count() != 0)
                            DeleteProject(item, projectExist.First(), db);

                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            onDBChange.ReleaseMutex();
            return true;
        }

        private static void DeleteProject(KeyValuePair<int, string> item, Projects project, KircheModelContext db)
        {
            var Projects_Projects_Type_forDelete = db.Projects_Project_Type
                .Where(elem => elem.project_id == item.Key
                    && elem.district_id == (from dis in db.District.AsQueryable()
                                            where dis.name == item.Value
                                            select dis.id).FirstOrDefault());

            db.Projects_Project_Type.RemoveRange(Projects_Projects_Type_forDelete);

            db.Projects.Remove(project);
        }
        #endregion
    }
}
