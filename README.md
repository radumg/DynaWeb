# DynWWW
[![Build Status](https://travis-ci.org/radumg/DynWWW.svg?branch=master)](https://travis-ci.org/radumg/DynWWW) [![GitHub version](https://badge.fury.io/gh/radumg%2FDynWWW.svg)](https://badge.fury.io/gh/radumg%2FDynWWW) [![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/radumg/DynWWW/blob/master/docs/CONTRIBUTING.md)
---
**DynWWW** is a [Dynamo](http://www.dynamobim.org) package providing support for interaction with the interwebz in general and with REST APIs in particular.

# Getting Started
TBC

# Using DynWWW
TBC

## Class structure
WIP

## Nodes
TBC

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

After you have the project saved to your development machine, navigate to the `DynWWW\src` folder and open the `DynWWW.sln` solution to access the full Visual Studio solution and source code. 

Build the project before making any changes to make sure the environment is properly set up, as the project relies on a few NuGet packages, see list below.

## Built with

The `DynWWW` project relies on a few community-published NuGet packages as listed below :
* [Newtonsoft](https://www.nuget.org/packages/newtonsoft.json/) - handles serializing and deserializing to JSON
* [RestSharp](https://www.nuget.org/packages/RestSharp/) - enables easier interaction with REST API endpoints
* [DynamoServices](https://www.nuget.org/packages/DynamoVisualProgramming.DynamoServices/2.0.0-beta4066) - an official Dynamo package providing support for better mapping of C# code to Dynamo nodes

## Contributing

Please read [CONTRIBUTING.md](https://github.com/radumg/DynWWW/docs/CONTRIBUTING.md) for details on how to contribute to this package. Please also read the [CODE OF CONDUCT.md](https://github.com/radumg/DynWWW/docs/CODE_OF_CONDUCT.md).

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
