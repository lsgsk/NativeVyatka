using System;
using Abstractions;
using System.Threading.Tasks;
using Android.Locations;
using System.Collections.Generic;
using System.Threading;
using Android.Text;
using Android.App;

namespace Plugins
{
    public class FetchAddressRealization : IFetchAddress
    {
        public async Task<string> GetAdress(double latitude, double longitude, CancellationToken? token = null)
        {
            String errorMessage = "";
            // Get the location passed to this service through an extra.
            Location location = new Location("gps") { Latitude = latitude, Longitude = longitude};
            var geocoder = new Geocoder(Application.Context, Java.Util.Locale.Default);
            IList<Address> addresses = null;
            try
            {
                // In this sample, get just a single address.
                addresses = await geocoder.GetFromLocationAsync(location.Latitude, location.Longitude, 1);
            }
            catch (Java.IO.IOException ioException)
            {
                // Catch network or other I/O problems.
                errorMessage = "service not available";
                Console.WriteLine(ioException.ToString());
            }
            catch (Java.Lang.IllegalArgumentException illegalArgumentException)
            {
                // Catch invalid latitude or longitude values.
                errorMessage = "invalid lat long used. " + "Latitude = " + location.Latitude + ", Longitude = " + location.Longitude; 
                Console.WriteLine(illegalArgumentException.ToString());
            }

            // Handle case where no address was found.
            if (addresses == null || addresses.Count == 0)
            {
                if (string.IsNullOrEmpty(errorMessage))
                {
                    Console.WriteLine("no_address_found");
                }
                deliverResultToReceiver(Constants.FAILURE_RESULT, errorMessage);
                return string.Empty;
            }
            else
            {
                Address address = addresses[0];
                var addressFragments = new Java.Util.ArrayList();

                // Fetch the address lines using getAddressLine,
                // join them, and send them to the thread.
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    addressFragments.Add(address.GetAddressLine(i));
                }
                Console.WriteLine("address_found");

                var result = TextUtils.Join(Java.Lang.JavaSystem.GetProperty("line.separator"), addressFragments);
                deliverResultToReceiver(Constants.SUCCESS_RESULT, result);
                return result;
            }
        }
        private void deliverResultToReceiver(int resultCode, String message) 
        {
            
        }

        public static class Constants
        {
            public const int SUCCESS_RESULT = 0;
            public const int FAILURE_RESULT = 1;
            public const string PACKAGE_NAME = "com.google.android.gms.location.sample.locationaddress";
            public const string RECEIVER = PACKAGE_NAME + ".RECEIVER";
            public const string RESULT_DATA_KEY = PACKAGE_NAME + ".RESULT_DATA_KEY";
            public const string LOCATION_DATA_EXTRA = PACKAGE_NAME + ".LOCATION_DATA_EXTRA";
        }

    }
}

