using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XamarinAzureMobileAppTestService.DataObjects
{
    // this partial class is compiled together with the one in the Shared project
    // it inherits EntityData, allowing it to be used by EntityFramework
    public partial class HelloItem : EntityData
    {
        public HelloItem()
        {
            //this.Name = "x"; // the property from the other part of this class should be visible here
        }
    }
}