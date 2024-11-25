# Timesheet Exercise

by Andrew Johnson

This is my implementation of the developer exercise described at https://github.com/ehsan-cmap/cmap-developer-exercise.
It was developed in using Visual Studio and C#. 

The solution contains two projects: TimeSheet.App and Timesheet.Tests. These are briefly described below.


## Approach

I tried to follow a test-driven approach for the development of the service. Test were defined in parallel
with adding endpoint stubs, which were ithen teratively enhanced together. Commenting is light, but I hope
describes the intention of the code. I built the service part of the App project first, followed by the UI
part.


## Timesheet.App

This is an ASP.NET Core MVC project that uses Entity Framework to provide an in-memory database for
storing timesheet entries. In the debugger the UI is exposed at http://localhost:7045/ and the service
endpoints are at http://localhost:7045/Home.

Two endpoints are provided:

- Add() for adding a timesheet entry to the in-memory database
- Timesheet() for getting the entries as a CSV file as specified in the problem statement

The HTML UI provides an HTML form for entering a timesheet entry, which is POSTed to
the Add() endpoint. It also provides a link that GETs the output from the Timesheet() endpoint
and uses some javascript to save it to the local filesystem.


## Timesheet.Tests

This is an MSTest project that implements tests for the Timesheet.App pproject - specifically for the
`HomeController` class. The tests use Moq and exercise the controller's two entry-points:

- Add() is tested with a valid entry, and a variety of invalid entries
- Timesheet() is tested using the example data specified in the problem statement
