# DynaWeb
[![Build Status](https://travis-ci.org/radumg/DynWWW.svg?branch=master)](https://travis-ci.org/radumg/DynWWW) [![GitHub version](https://badge.fury.io/gh/radumg%2FDynaWeb.svg)](https://badge.fury.io/gh/radumg%2FDynaWeb) [![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/radumg/DynaWeb/blob/master/CONTRIBUTING.md)
---
__DynaWeb__ is a [Dynamo](http://www.dynamobim.org) package providing support for interaction with the interwebz in general and with REST APIs in particular.

It's meant to provide building blocks so you can build Dynamo integrations with just about any web service out there. It helps you retrieve (GET) stuff from the web, send (POST) information to the web and everything in between (PUT, DELETE, PATCH, etc). The package also includes some rather nifty `deserialisation` nodes, so you can use the information you get from the web directly in Dynamo graphs, as native types.

![DynaWeb package screenshot](https://raw.githubusercontent.com/radumg/DynaWeb/master/samples/DynaWeb.png)

# Status : Beta
The package is in active development and is currently available as an open beta.

This stage is for intense feedback from you, testing it, so I would really like to know what cloud/web platforms you'd like to use Dynamo with and how.
Please see and add your comments to this [first issue](https://github.com/radumg/DynaWeb/issues/1) and feel free to [open more issues](https://github.com/radumg/DynaWeb/issues/new) with any ideas you might have. 

## Repository notes
This public repository is only hosting the issues and in-progress builds of the package, accessible on the [releases page](https://github.com/radumg/DynaWeb/releases).
The repository is only private in the sense that source code is not made available and contributions are by invite-only. If you would like to contribute and help develop it, please open an issue requesting access to the `development repo` instead.

*Note : I have every intention to open-source this eventually but would like to focus on testing and validating some of the underlying assumptions before making it available to the general audience. The ultimate goal is to see this merged back into the Dynamo Core library, to enable as many people to start interacting with the web more.*

*I know all this is slightly different to how i usually do things (oss), but i wanted to test a few things out. Feedback is welcomed, as always.*


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

# Using DynaWeb
Please consult the sample files provided in this repository and in the package's `extra` folder - they contain notes and instructions on how to use the nodes. I'm not providing extensive documentation on this page on purpose - to see how intuitive the design of the package & nodes is to first-time users.

Feel free to submit PR if you want to add some documentation in the meantime.

## Samples
There are 7 sample Dynamo graphs included with the package, check out the `extra` folder in the downloaded package or the [samples folder](https://github.com/radumg/DynaWeb/tree/master/samples) of this repository. Also note the DYN samples are offered in both Dynamo 1.3 file format and Dynamo 2.0 new format.

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

__Sample 6 - Comples POST request__
further expands on the above example, building a complex `WebRequest` with 6 steps before its execution.

__Sample 7 - Upload file to Autodesk Forge__
this example builds a `WebRequest` and attaches a file to it, to upload directly to the `Autodesk Forge` service. See the issue that sparked this sample [here](https://github.com/radumg/DynaWeb/issues/11).


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


## Prerequisites

This project requires the following applications or libraries be installed :

```
Dynamo : version 1.3 or later
```
```
.Net : version 4.5 or later
```

Please note the project has no dependency to Revit and its APIs, so it will happily run in Dynamo Sandbox or Dynamo Studio.


## Built with

The `DynaWeb` project relies on a few community-published NuGet packages as listed below :
* [Newtonsoft](https://www.nuget.org/packages/newtonsoft.json/) - handles serializing and deserializing to JSON
* [RestSharp](https://www.nuget.org/packages/RestSharp/) - enables easier interaction with REST API endpoints
* [DynamoServices](https://www.nuget.org/packages/DynamoVisualProgramming.DynamoServices/2.0.0-beta4066) - an official Dynamo package providing support for better mapping of C# code to Dynamo nodes

## Contributing

Please read [CONTRIBUTING.md](https://github.com/radumg/DynWWW/blob/master/docs/CONTRIBUTING.md) for details on how to contribute to this package. Please also read the [CODE OF CONDUCT.md](https://github.com/radumg/DynWWW/blob/master/docs/CODE_OF_CONDUCT.md).

## Authors

__Radu Gidei__ : [Github profile](https://github.com/radumg), [Twitter profile](https://twitter.com/radugidei)

## License

This project is licensed under the GNU AGPL 3.0 License - see the [LICENSE FILE](https://github.com/radumg/DynWWW/blob/master/LICENSE) for details.
