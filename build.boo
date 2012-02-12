solution_file = "src/SimpleService.sln"
configuration = "release"
version = env("version")
build_name = ""

if version != null:
  build_name = "SimpleService-" + version
else:
  build_name = "SimpleService"

build_dir = "build/${build_name}"

target default, (init, compile, deploy, package, nuget):
  pass

target init:
  rmdir(build_dir)
  
desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration)
  
desc "Copies the binaries to the 'build' directory"
target deploy:
  print "Copying to build dir"

  with FileList("src/SimpleService/bin/${configuration}"):
    .Include("*.{dll,exe}")
    .ForEach def(file):
      file.CopyToDirectory(build_dir + "/SimpleService")
  
  print "Copying libs to build dir"
  
  with FileList("lib"):
    .Include("*.{dll}")
    .ForEach def(file):
      file.CopyToDirectory(build_dir + "/SimpleService")
      
  print "Copy readme file to build dir"
  
  cp("README.md", build_dir + "/SimpleService/README.txt")
      
desc "Creates zip package"
target package:
  zip(build_dir, build_dir + "/" + build_name + '.zip')

desc "Making nuget-package"
target nuget, package:
  with FileList(build_dir + "/SimpleService"):
    .Include("*.{dll,exe}")
    .ForEach def(file):
      file.CopyToDirectory("src/NuGetPackage/lib/net40")
    
  with nuget_pack():
    .toolPath = "tools/nuget/nuget.exe"
    .nuspecFile = "src/NuGetPackage/simpleservice.nuspec"
    .outputDirectory = build_dir
    .basePath = "src/NuGetPackage"
    .version = version