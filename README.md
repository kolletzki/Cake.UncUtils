# Cake.UncUtils

This is a collection of UNC aliases for [Cake](http://cakebuild.net).

## Usage
```csharp
#addin nuget:?package=Cake.UncUtils

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

- [x] Use Cake for builds
- [x] Create NuGet package
- [ ] Support file links
- [ ] Enhance tests