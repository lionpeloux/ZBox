# Welcome to ZBox !

## What is ZBox ? 

Well, `ZBox` stands for `ZymeToolbox`. `Zyme` is the name of my company. 

It is a collection of usefull pieces of code written in `C#` that I want to be reusable across my projects and, possibly, easy to share to others.

## What's the architecture of ZBox ? 

For now, `ZBox` is composed of to main projects :

* `ZymeToolbox.Core` : the core library as a reusable / protable `.Net Standard 2.0` class library with as few dependencies as possible.
* `ZymeToolbox.Grasshopper` : a grasshopper plugin that exposes the key core library logic through components.

## Documentation for the Grasshopper Plugin

### How to install ?

Just download the last release and copy/past the .gha file(s) into your Grasshopper `/Library` special folder

### Weather Data API

These components allows to query directly existing Weather Data Files availbale through the folowing providers :
* [EnergyPlus.Net](https://energyplus.net/weather)
* [Climate.OneBuilding.Org](https://climate.onebuilding.org)

### PVGIS API

A set of components that wraps the [PVGIS Tools](https://re.jrc.ec.europa.eu/pvg_tools/en/) by making direct calls to the underlying REST API.

`PVGIS` provides information about solar radiation and photovoltaic (PV) system performance for any location in Europe and Africa, as well as a large part of Asia and America. You 'll get free and open access to:

* PV electricity generation potential for different technologies and configurations
* Solar radiation and temperature, as monthly averages or daily profiles
* Full time series of hourly values of both solar radiation and PV performance
* TMY data for nine climatic variables, formatted for building energy calculation tools
