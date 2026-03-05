

This repository contains the full-stack implementation for authenticating, translating, and viewing 3D/2D BIM models (Revit, AutoCAD, etc.) using Autodesk Platform Services (APS).

 Educational Context
This project is part of a specialized tutorial series for AEC & BIM Automation engineers. It demonstrates how to bridge the gap between desktop BIM software and cloud-based web applications.

 Key Features Covered:
OAuth 2.0 Authentication: Secure 2-legged and 3-legged authentication flows.

Data Management API: Programmatic Bucket creation and file uploading to OSS.

Model Derivative API: Converting raw .rvt or .dwg files into SVF2 format.

APS Viewer (Front-end): A high-performance 3D viewer with property inspection and navigation tools.

 Tech Stack
Back-end: .NET Core Web API (C#)

Front-end: HTML5, CSS3, Vanilla JavaScript (APS Viewer SDK v7.*)

Development Tools: Visual Studio 2022 & VS Code

 How to Run This Project
1. Prerequisites
An active APS (Forge) Account. Create one at aps.autodesk.com.

Client ID and Client Secret from your APS App.

.NET SDK 6.0 or later.

2. Configuration (Back-end)
Open the solution in Visual Studio.

Update your ClientId and ClientSecret in the AuthController.cs.

Ensure your Callback URL in the APS Dashboard matches:
https://localhost:7244/api/auth/callback

Run the API project.

3. Configuration (Front-end)
Open the Front-end folder in VS Code.

In viewer-logic.js, ensure the fetch URL points to your running Localhost API (e.g., https://localhost:7244).

Launch index.html using Live Server.

 Critical Technical Highlights
 Security (The Public Token)
To prevent exposing sensitive credentials, the Viewer requests a Public Token from our back-end. This token only has viewables:read scope, ensuring users can view the model but cannot delete or modify files in your Bucket.

CORS Policy
Since the Front-end and Back-end run on different ports, a CORS Policy is implemented in Program.cs to allow the browser to securely fetch the access token from the API.

 URN Encoding
The project includes a utility to convert raw Object IDs into Base64 URL-Safe strings, which is mandatory for the Model Derivative service to locate the file.

 Learning Resources
Full Video Tutorial: https://www.youtube.com/playlist?list=PL2ZMKOxAeBJEOcSMwzmS3b05wQ1QNCxQ1

Official Documentation: APS Developer Guide
