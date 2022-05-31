# Template Service for .NET Core Web API

This is a template that can be used to quickly create .NET Core Web API projects. This template helps you create a dockerized web api service with Prometheus metrics, health checks and a MySql database backend. It also contains Serilog logging to the console.

## Pre-requisites

Ensure that you have .NET 6.0 or later installed along with Docker Desktop

## Getting Started

To create your own project from this template, follow the steps outlined here:

1. Create an empty git project and initialize it.
2. Clone this project and copy the contents to your new git project (take care not to copy the `.git` folder)
3. Open the new project folder in Windows Explorer (or Mac Finder) and rename all folders from `TemplateService` to your new project name (for example, `MyService`)
4. Open the project in an editor like Visual Studio Code, and do a global search and replace - replace `TemplateService` with `MyService`
5. From the root folder of your repository, run `dotnet build`. Resolve any build failures and you should now have a basic .NET Core Web Api up and running.

**Do NOT use the IDE's `Refactor` command to do the above steps! Instead, use the Search & Replace in files command**

## Other things to consider

1. This sample web api is designed to run in a container using an orchestration layer like Kubernetes. Therefore, it does not enable HTTPS with the assumption being that HTTPS traffic will be terminated at the Kubernetes cluster ingress point
2. This service requires an Okta Access Token for any Controller or Method marked with [Authorize] annotation. This can be replaced by other authentication services, for example, with Auth0
3. If not running in a Kubernetes cluster, use `app.UseHsts()` in the Configure method
