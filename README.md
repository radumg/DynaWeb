# DynaWeb
[![Build Status](https://travis-ci.org/radumg/DynWWW.svg?branch=master)](https://travis-ci.org/radumg/DynWWW) [![GitHub version](https://badge.fury.io/gh/radumg%2FDynaWeb.svg)](https://badge.fury.io/gh/radumg%2FDynaWeb) [![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/radumg/DynWWW/blob/master/docs/CONTRIBUTING.md)
---
__DynaWeb__ is a [Dynamo](http://www.dynamobim.org) package providing support for interaction with the interwebz in general and with REST APIs in particular.

### What does it do ?

It helps you 
- retrieve (GET) stuff from the web
- send (POST) information to the web
- everything in between (PUT, DELETE, PATCH, etc).
- the package also includes some rather nifty JSON `de/serialisation` nodes, so you can use the information you get from the web directly in Dynamo graphs, as native types.

### How it came about

__DynaWeb__ was designed as a package to make other packages, so it provides building blocks enabling you to build Dynamo integrations with just about any web service out there. After making DynaSlack & DynAsana, it became clear that writing a ZeroTouch-based package for every web service I or the community would want to integrate with was simply not scalable or sustainable, no matter how much code was re-used. DynAsana is an abstracted DynaSlack and DynaWeb is an even more abstracted & modularised DynAsana.

## Next steps

The project's aspiration is to merge the functionality of DynaWeb back into the core library of Dynamo. Progress & steps required are tracked in the [2.0 Integration into Core](https://github.com/radumg/DynWWW/projects/1) project.

Why core and not leave it as a package? As the industry starts leveraging cloud platforms more and more, it would be great to make interacting with the web a first-class citizen in the Dynamo ecosystem. From my own observations, the more experienced or adventurous users are already leveraging the Dynamo package to connect to cloud apps, for use-cases ranging from data interop, analytics, notifications, federation, etc.
I think Dynamo can have a big role to play in accelerating this and enabling people to build connected systems, but to do that, you need easy-to-use nodes as part of the core library that everyone can rely on. I'm sure this is being tackled and addressed with AVP, but in the meantime and for users who won't be ready/aren't allowed to migrate their entire workflow & data to Forge, `Dynamo 2.x` will still be a reference application. 

### Challenges
This brings me to a few identified challenges in making this part of the core library. Please visit each issue below to contribute to the discussion :

- Verbosity / Atomicitiy : the package has many nodes and building a complex request can require chaining as many as 10 nodes. Whilst this is great for flexibility and power-users, it's intimidating and creates a steep learning curve for new users. See the [issue](https://github.com/radumg/DynWWW/issues/40)

- Caching & Repeated execution : repeated execution of the same graph can have unintended consequences and may send malformed requests (doubled up headers for example) or fail to send an identical request a second time. These shortcomings can be tackled by using `NodeModels` so a discussion around this is needed, see the [issue](https://github.com/radumg/DynWWW/issues/41)

![DynaWeb package screenshot](https://raw.githubusercontent.com/radumg/DynaWeb/master/samples/DynaWeb.png)

## Class structure
There's 5 main namespaces you'll find in DynaWeb : 
- `WebRequest` : the web request that gets executed
- `WebClient` : the context in which a request is executed
- `WebResponse` : this contains the response from the server, as well as additional metadata about the response & server itself 
- `Execution` : this provides nodes that simply execute requests, making it easier & clearer to use standard http verbs such as GET, POST, etc.
- `Helpers` : a few helper nodes, with a particular focus on `Deserialisation.`

Simply put, use `WebRequest` nodes for one-off requests and start using a `WebClient` when you are interacting with REST APIs and/or have multiple request to similar endpoints/URLs.
When using a `WebClient`, the `WebRequest` is still what gets executed, but it allows you more control over how that occurs (custom timeouts, etc)

*Note : When executing a `WebRequest` on its own, the DynaWeb package constructs an empty `WebClient` in the background anyway as it's needed for execution.*

# Getting Started

## Package manager
`DynaWeb` is now available on the Dynamo package manager, search for `DynaWeb` and install it. 

## Manual install
If you prefer to install one of the more experimental/work-in-progress builds, you can still follow the instructions below.

- Download the latest release from the [Releases page](https://github.com/radumg/DynaWeb/releases)
- unzip the downloaded file
- once unzipped, copy the `DynaWeb` folder to the location of your Dynamo packages  :
    - `%appdata%\Dynamo\Dynamo Core\1.3\packages` for Dynamo Sandbox, replacing `1.3` with your version of Dynamo
    - `%appdata%\Dynamo\Dynamo Revit\1.3\packages` for Dynamo for Revit, replacing `1.3` with your version of Dynamo
- start Dynamo, the package should now be listed as `DynWWW` in the library.

## Still can't see the package in Dynamo ?

This issue should be fixed now the package is distributed through the package manager, I definitely recommending getting it that way. However, in case you still have issues, see instructions below :

As [reported](https://github.com/radumg/DynaWeb/issues/10) by users, Windows sometimes blocks `.dll` files for security reasons. To resolve this, you'll have to go through the steps below for each assembly (`.dll` file) in the package :
  1. Right-click on `.dll` file and select properties
  2. Tick the `Unblock` checkbox at the bottom, in the Security section.
  3. Launch Dynamo again, the package should now load.

![image](https://user-images.githubusercontent.com/15014799/29770289-3c13172a-8be6-11e7-983e-6fb3c71ad136.png)

## Updating from alpha-0.5 build ?
The changes in `1.0` are breaking, meaning graphs using the previous version will not work. However, instead of re-creating them, you can simply open the `.dyn` files using Notepad (though i recommend SublimeText) and perform the following text find/replaces :
- replace `DSCore.Web.` with `DynaWeb.`
- replace `DynWWW.dll` with `DynaWeb.dll`
- replace `WebClient.WebClient` with `WebClient.ByUrl`

# Using DynaWeb
Please consult the sample files provided in this repository and in the package's `extra` folder - they contain notes and instructions on how to use the nodes. I'm not providing extensive documentation on this page on purpose - to see how intuitive the design of the package & nodes is to first-time users.

Feel free to submit PR if you want to add some documentation in the meantime.

## Samples
There are 8 sample Dynamo graphs included with the package, check out the `extra` folder in the downloaded package or the [samples folder](https://github.com/radumg/DynaWeb/tree/master/samples) of this repository. Also note the DYN samples are offered in both Dynamo 1.3 file format and Dynamo 2.0 new format.

The samples start from super-simple and increase in complexity :

__Sample 1 - A first request__
3 nodes, similar to out-of-the-box (OOTB) Dynamo experience today.

__Sample 2 - A simple request__
introduces the 3 stages of performing web requests and explains quite a few things. Also show how to achieve same thing with the OOTB node.

__Sample 3 - Requst + benchmarking__
same as sample 2 but with added nodes that provide more information about the request (timing, etc) and output the results to text files.

__Sample 4 - REST API example__
this introduces the use of the `WebClient` class and some of the basic priciples of interacting with REST services. Uses a REST API that is freely accessible and returns JSON reponses. Contrasts using a `WebClient` and a `WebRequest` to achieve same thing and also introduces `Deserialisation`.

__Sample 5 - REST API advanced__
Introduces POST-ing to a REST API service and handling JSON payloads. Once the request is submitted, the response is deserialised too.

__Sample 6 - Complex POST request__
further expands on the above example, building a complex `WebRequest` with 6 steps before its execution.

__Sample 7 - Upload file to Autodesk Forge__
this example builds a `WebRequest` and attaches a file to it, to upload directly to the `Autodesk Forge` service. See the issue that sparked this sample [here](https://github.com/radumg/DynaWeb/issues/11).

__Sample 8 - Autodesk Forge request token__
this example builds a POST `WebRequest`, used to request an authorisation token from the `Autodesk Forge` service. See the issue that sparked this sample [here](https://github.com/radumg/DynaWeb/issues/13).


## Prerequisites

This project requires the following applications or libraries be installed :

```
Dynamo : version 1.3 or later
```
```
.Net : version 4.5 or later
```

Please note the project has no dependency to Revit and its APIs, so it will happily run in Dynamo Sandbox or Dynamo Studio.

# Usage of this repository & source code

The below sections detail how to use this repository, contribute to it and any licensing restrictions.

## Get your development copy of DynWWW

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

As a prerequisite, this project was authored in and requires :
```
Visual Studio : version 2017 or later
```

Visual Studio 2015 should also work - but it's not supported so keep that in mind before creating an issue or submitting a PR.

If you already have a Github account, fork (& star) the project, then `clone to desktop` or `clone using Github Desktop`.

If you don't already have a Github account or don't want one (why wouldn't you ?), follow these steps :
```
- click the green `Clone or Download` button on this project's page and select `Download ZIP`
- unzip the downloaded file
- to enable testing in Dynamo, copy the `packages\DynWWW` folder to the location of Dynamo packages  :
    - `%appdata\Dynamo\Dynamo Core\1.2\packages` for Dynamo Sandbox, replacing `1.2` with your version of Dynamo
    - `%appdata\Dynamo\Dynamo Revit\1.2\packages` for Dynamo for Revit, replacing `1.2` with your version of Dynamo
```

After you have the project saved to your development machine, navigate to the `DynWWW\src\DynWWW\` folder and open the `DynWWW.sln` solution to access the full Visual Studio solution and source code. 

Build the project before making any changes to make sure the environment is properly set up, as the project relies on a few NuGet packages, see list below.

## Built with

The `DynWWW` project relies on a few community-published NuGet packages as listed below :
* [Newtonsoft](https://www.nuget.org/packages/newtonsoft.json/) - handles serializing and deserializing to JSON
* [RestSharp](https://www.nuget.org/packages/RestSharp/) - enables easier interaction with REST API endpoints
* [DynamoServices](https://www.nuget.org/packages/DynamoVisualProgramming.DynamoServices/2.0.0-beta4066) - an official Dynamo package providing support for better mapping of C# code to Dynamo nodes

## Contributing

Please read [CONTRIBUTING.md](https://github.com/radumg/DynWWW/blob/master/docs/CONTRIBUTING.md) for details on how to contribute to this package. Please also read the [CODE OF CONDUCT.md](https://github.com/radumg/DynWWW/blob/master/docs/CODE_OF_CONDUCT.md).

## Versioning & Releases

DynAsana use the [SemVer](http://semver.org/) semantic versioning standard.
For the versions available, see the versions listed in the Dynamo package manager or [releases on this repository](https://github.com/radumg/DynWWW/releases).
The versioning for this project is `X.Y.Z` where
- `X` is a major release, which may not be fully compatible with prior major releases
- `Y` is a minor release, which adds both new features and bug fixes
- `Z` is a patch release, which adds just bug fixes

examples :
```
major : changing the namespace, name of a base class
minor : adding a new node
patch : fixing a bug with an existing node
```

Please refer to the above when submitting a PR.

## Authors

* **Radu Gidei** : [Github profile](https://github.com/radumg), [Twitter profile](https://twitter.com/radugidei)

## License

This project is licensed under the GNU AGPL 3.0 License - see the [LICENSE FILE](https://github.com/radumg/DynWWW/blob/master/LICENSE) for details.

## Acknowledgments

* Hat tip to the [UK Dynamo User Group](http://www.twitter.com/ukdynug) and the wider [Dynamo community](http://www.dynamobim.org) for spurring me on to present & release this.

* The codebase is in no way striving for efficiency, but instead simplicity & legibility for the community's benefit - hence the many comments left throughout the code. Any suggestions or help improving it is warmly welcomed.
