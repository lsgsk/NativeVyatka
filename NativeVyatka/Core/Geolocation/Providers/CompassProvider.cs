using System;
using Android.Content;
using Android.Hardware;
using Android.Runtime;

namespace NativeVyatka
{
    public interface ICompassProvider
    {
        event EventHandler<double> OnDegreeChanged;
        void StartCompassMonitoring();
        void StopCompassMonitoring();
    }

    public class CompassProvider: Java.Lang.Object, ISensorEventListener, ICompassProvider
    {
        private readonly SensorManager sensorManager;
        public event EventHandler<double> OnDegreeChanged;

        public CompassProvider(Context context) {
            this.sensorManager = (SensorManager)context.GetSystemService(Context.SensorService);
        }

        public void StartCompassMonitoring() {
            sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Orientation), SensorDelay.Game);
        }

        public void StopCompassMonitoring() {
            sensorManager.UnregisterListener(this);
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy) {
        }

        public void OnSensorChanged(SensorEvent e) {
            OnDegreeChanged?.Invoke(this, Math.Round(e.Values[0]));
        }
    }
}
