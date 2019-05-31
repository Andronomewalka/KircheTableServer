using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransitPackage
{
    public enum ActionEnum
    {
        login, get_all,
        update_send, update_receive,
        del_send, del_receive,
        logout_receive, get_categories,
        connection_check
    }

    public enum OperationResult { Good, Bad}
}
