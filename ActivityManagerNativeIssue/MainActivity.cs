using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.Res;
using Java.Util;
using Java.Lang.Reflect;

namespace ActivityManagerNativeIssue
{
    [Activity(Label = "ActivityManagerNativeIssue", MainLauncher = true)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button btn = FindViewById<Button>(Resource.Id.btn);
            btn.Click += (sender, e) =>
            {
                var amnClass = Java.Lang.Class.ForName("android.app.ActivityManagerNative");
                Java.Lang.Object amn = null;
                Configuration config = null;

                var methodGetDefault = amnClass.GetMethod("getDefault");
                methodGetDefault.Accessible = true;
                amn = methodGetDefault.Invoke(amnClass);

                if (Build.VERSION.SdkInt >= Build.VERSION_CODES.O)
                {
                    amnClass = Java.Lang.Class.ForName(amn.Class.Name);
                }

                var methodGetConfiguration = amnClass.GetMethod("getConfiguration");
                methodGetConfiguration.Accessible = true;
                config = methodGetConfiguration.Invoke(amn) as Configuration;

                var configClass = config.Class;
                var f = configClass.GetField("userSetLocale");
                f.SetBoolean(config, true);

                Locale locale = new Locale("zh", "CN");

                config.Locale = locale;

                var methodUpdateConfiguration = amnClass.GetMethod("updateConfiguration", Java.Lang.Class.FromType(typeof(Configuration)));
                methodUpdateConfiguration.Accessible = true;
                try
                {
                    methodUpdateConfiguration.Invoke(amn, config);
                }
                catch (InvocationTargetException ee)
                {
                    var error = ee.TargetException;
                }
            };
        }
    }
}