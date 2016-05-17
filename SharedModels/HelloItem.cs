using System;

namespace XamarinAzureMobileAppTestService.DataObjects
{
    // This is a partial class in a Shared project
    // the Shared project doesn't compile on its own but rather 
    // serves as a container for files that compile as a part of another project
    // This allows the important parts of the model to be placed in a single location
    // and in other projects a counterpart partial class can inherit from any base class
    public partial class HelloItem
    {
        public string Name { get; set; }

        public string Location { get; set; }
    }
}