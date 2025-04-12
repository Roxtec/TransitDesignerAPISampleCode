# Transit Designer API sample code

This repository contains code samples for [Roxtec Transit Designer<sup>TM</sup>](https://www.roxtec.com/en/service-support/design/roxtec-transit-designer/).

## Repository structure

The repository is structured as follows:

```
samples/
\--<api name>
   \--<language>
      |-- <sample folder 1>
      |-- ...
      \-- <sample folder N>
```

Currently, there is a single API: _transit_layouts_

Similarly, all code samples are in C#, under the _csharp_ language folder.

## How to use the samples

### .NET version

The C# code makes use of .NET 9.0. It can be installed
from [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).

### Swagger

The Swagger documentation for Transit Designer can be viewed [here](https://transitdesigner.roxtec.com/api/docs).
This link always refers to the latest API documentation.

The Swagger documentation exists in a machine-readable file (swagger.json) as well, which we will
use in the next section. 

This file is located at: https://transitdesigner.roxtec.com/api/docs/swagger.json

### Generate a Swagger client

To generate a Swagger client for C#, please complete the following steps.
For the samples to work, the current directory should be the root of this
Git repository.

Here we use [NSwag](https://github.com/RicoSuter/NSwag) to generate the client.
It can be done in many different ways (see the NSwag link). Here we do it with an npm
package called `nswag`. Note that this requires Node.js and the `npx` to be installed.

1. Check if you have node installed:

        node -v

    It should return something like (or a newer version number):

        > node -v
        v20.18.0

    If it cannot be found, head over to: https://nodejs.org/en/download/
    and download an installation.

2. Install npx

        npm install -g npx

3. Run the following command from a PowerShell or command prompt:

        npx nswag openapi2csclient /Namespace:RtdApiCodeSamples /Input:https://transitdesigner.roxtec.com/api/docs/swagger.json /Output:Full.cs

4. Verify the the file `Full.cs` has been written.

### Project ID and API key

All samples must be run with a Transit Designer project ID. To obtain the ID,
open Transit Designer and navigate to the overview of a project (create one if
necessary). The URL should look as follows:

`https://transitdesigner.roxtec.com/app/projects/1d64040c-c5e2-4c8c-9be4-281bf96a144c/overview`

The project ID in this case is `1d64040c-c5e2-4c8c-9be4-281bf96a144c`.

Some samples operate one one or more transits. A transit ID can be extracted from the
URL as well. Navigate to the cable registration page of a transit. The URL should
look as follows:

`https://transitdesigner.roxtec.com/app/projects/1d64040c-c5e2-4c8c-9be4-281bf96a144c/transits/3502645f-1693-4729-bd15-069366b78144/cablereg`

The transit ID in this case is `3502645f-1693-4729-bd15-069366b78144`.

Furthermore, it is necessary to have an API key. Go to the project settings, then
choose the _API key_ tab. If this tab is not visible, you do not have access to
Transit Designer APIs. Please contact a Roxtec representative to request access.

On the _API key_ tab, follow the instructions to generate a project API key.
Please copy and store the key in a safe place; once it has been generated, it
cannot be obtained from Transit Designer again.

### Running a sample

> Note that the _VisitRtd_ sample has a separate README file inside its sample folder,
  as its build and run processes differ from the other samples.

The C# samples are made to be run using `dotnet run`. Each sample has its own
folder to facilitate this.

To run an individual sample, navigate to its directory and run:

    dotnet run [arguments...]

For example, to create a transit:

    cd samples
    cd transit_layouts
    cd csharp
    cd Create
    dotnet run <project ID> <project API key>

Replace `<project ID>` and `<project API key>` with the project ID and API key,
respectively. 

All samples require project ID and API key to be passed on the command line.
One or more transit IDs can optionally be passed after the API key. If not,
the sample program will ask for transit ID(s) if necessary.