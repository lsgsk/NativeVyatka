using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace NativeVyatka
{
    public interface IPermissionsProvider
    {
        Task RequestPermissionAsync();
        Task<bool> IsLocationPermissionGrantedAsync();
        Task<bool> IsCameraPermissionGrantedAsync();
        Task<bool> IsStoragePermissionGrantedAsync();
    }

    public class PermissionsProvider : IPermissionsProvider
    {
        private readonly IPermissions permissions;

        public PermissionsProvider(IPermissions permissions) {
            this.permissions = permissions;
        }

        public async Task RequestPermissionAsync() {
            try {
                var result = true;
                if (await permissions.CheckPermissionStatusAsync<CameraPermission>() != PermissionStatus.Granted &&
                    await permissions.RequestPermissionAsync<CameraPermission>() != PermissionStatus.Granted) {
                    result = false;
                }
                if (await permissions.CheckPermissionStatusAsync<StoragePermission>() != PermissionStatus.Granted &&
                    await permissions.RequestPermissionAsync<StoragePermission>() != PermissionStatus.Granted) {
                    result = false;
                }
                if (await permissions.CheckPermissionStatusAsync<LocationPermission>() != PermissionStatus.Granted &&
                    await permissions.RequestPermissionAsync<LocationPermission>() != PermissionStatus.Granted) {
                    result = false;
                }
                if (result == false) {
                    throw new PermissionsException();
                }
            }
            catch {
                throw new PermissionsException();
            }
        }

        public async Task<bool> IsLocationPermissionGrantedAsync() {
            try {
                return await permissions.CheckPermissionStatusAsync<LocationPermission>() == PermissionStatus.Granted;
            }
            catch {
                return false;
            }
        }
        public async Task<bool> IsCameraPermissionGrantedAsync() {
            try {
                return await permissions.CheckPermissionStatusAsync<CameraPermission>() == PermissionStatus.Granted;
            }
            catch {
                return false;
            }
        }
        public async Task<bool> IsStoragePermissionGrantedAsync() {
            try {
                return await permissions.CheckPermissionStatusAsync<StoragePermission>() == PermissionStatus.Granted;
            }
            catch {
                return false;
            }
        }
    }
}
