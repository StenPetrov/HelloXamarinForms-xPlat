using System;
using Microsoft.WindowsAzure.MobileServices;

namespace HelloForms
{
    public class MobileAppClient
    {
        public static MobileServiceClient Client =
            new MobileServiceClient (
            "https://xamarinazuremobileapptest.azurewebsites.net"
        );
    }
}

