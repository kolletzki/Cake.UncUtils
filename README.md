# Cake.UncUtils

This is a collection of UNC aliases for [Cake](http://cakebuild.net).

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

- [ ] Use Cake for builds
- [ ] Create NuGet package
- [ ] Support file links
- [ ] Enhance tests