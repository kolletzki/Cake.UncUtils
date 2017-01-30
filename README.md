# Cake.UncUtils

This is a collection of UNC extensions for [Cake](http://cakebuild.net).

## Usage
```csharp
#r "../path/to/Cake.UncUtils.dll"

var uncSource = @"\\SOMESERVER\somedir";
var localTarget = @"C:\Users\SomeUser\somedir\SOMESERVER";

Task("Demo")
	.Does(() => 
{
	MountUncDir(uncSource, localTarget);
	//Do something on path
	UnmountUncDir(localTarget);
});
```

# Todo

- [ ] Create NuGet package
- [ ] Support file links
- [ ] Enhance tests