# FileShareConnectivity

**Note:** The current file sharing implementation is only for Android-to-Android devices.

## Android

Add the following permissions to your `AndroidManifest.xml`. 
For Marshmallow and above, please follow [Requesting Runtime Permissions in Android Marshmallow](https://devblogs.microsoft.com/xamarin/requesting-runtime-permissions-in-android-marshmallow/) and don't forget to prompt the user for the location permission.

```xml
<uses-permission android:name="android.permission.ACCESS_WIFI_STATE" />
<uses-permission android:name="android.permission.CHANGE_WIFI_STATE" />
<uses-permission android:name="android.permission.CHANGE_NETWORK_STATE" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<!-- If any feature in your app relies on precise location information, don't include the "maxSdkVersion" attribute. -->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" android:maxSdkVersion="32" />
```

If your app targets Android 13 (API level 33) or higher, you must declare the NEARBY_WIFI_DEVICES permission.

```xml
<!-- If your app derives location information from Wi-Fi APIs, don't include the "usesPermissionFlags" attribute. -->
<uses-permission android:name="android.permission.NEARBY_WIFI_DEVICES" android:usesPermissionFlags="neverForLocation" />
```

Also, make sure to add the following features to your AndroidManifest.xml file:

```xml
<uses-feature android:name="android.hardware.wifi" android:required="true" />
<uses-feature android:name="android.hardware.wifi.direct" android:required="true" />
```

## Usage

### ShareActivity

Create a class under the Android platform for the ShareActivity that drive from BaseShareActivity. Make sure to trigger the base class in `OnCreate`:

```csharp
[IntentFilter(new[] { Intent.ActionSend }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "*/*")]
class ShareActivity : BaseShareActivity
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
		base.OnCreate(savedInstanceState);
		// ...
	}
}
```

### MauiProgram
Add the following to the MauiProgram.cs:

```csharp
var builder = MauiApp.CreateBuilder();
builder.UseFileShareConnectivity();
```

### Main API
Use FileSharingWrapper for the main API.

## Authors

- [@neta](https://www.github.com/Neta-007)
- [@avi](https://www.github.com/avim97)

# License
This project is licensed under the [LICENSE NAME] license.