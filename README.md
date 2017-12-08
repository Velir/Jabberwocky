# Overview

Jabberwocky is Velir's answer to the age-old adage: don't repeat yourself (We too, like to keep things DRY). It's a collection of loosely coupled modules intended to help speed up development on the Sitecore platform... and just generally make developer's lives easier while doing it.

Much as Helix has done for Sitecore solution architecture, Jabberwocky was also built to help push developers into the pit of success. It's opinionated in that it has a set of base libraries that it works well with, but it doesn't force you to use anything you don't want to.

## What's in the box?
Jabberwocky comes built-in with accelerators for the following technologies:

* **Dependency Injection** via **Autofac**
* **Sitecore Renderings** using **MVC**
* **ORM** models with **Glass Mapper**
* **Integrated Performance & Profiling** tooling with **MiniProfiler**
* **Transient Error Handling** strategies with **Polly**

## Quickstart

If you just want to get up and running with everything, you can install the meta-package from NuGet (coming-soon):

`Install-Package Jabberwocky.Library`

Otherwise, you can pick and choose which packages you want to use in your solution. The minimum requirements for version 2 of Jabberwocky are:

* Sitecore 8.2 (initial release)
* .NET 4.5.2

## Documentation

You can find the documentation for Jabberwocky here: https://jabberwocky.readthedocs.io/

Docs are in the process of being updated for version 2, so look out for the changes!
