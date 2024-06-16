![Build Status](https://github.com/johnhinz/AIGatekeeper/workflows/.NET%20Core/badge.svg?branch=master) [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/johnhinz/AIGatekeeper/blob/master/LICENSE) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=johnhinz_AIGatekeeper&metric=alert_status)](https://sonarcloud.io/dashboard?id=johnhinz_AIGatekeeper)

[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=johnhinz_AIGatekeeper&metric=bugs)](https://sonarcloud.io/dashboard?id=johnhinz_AIGatekeeper) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=johnhinz_AIGatekeeper&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=johnhinz_AIGatekeeper) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=johnhinz_AIGatekeeper&metric=code_smells)](https://sonarcloud.io/dashboard?id=johnhinz_AIGatekeeper)

[![Discord Chat](https://img.shields.io/badge/chat-on%20discord-7289DA.svg)](https://discord.gg/5kTurQfz)

# AIGateKeeper

The goal of this library is to design a tool that would watch a folder for specific files, send them to an AI discovery tool and take some action if an appropriate subject is found.
The library has abstracted the AI discovery tool and the repository.  There are implementations for [DeepStack](https://deepstack.cc/)  and [MQTT](https://github.com/chkr1011/MQTTnet).

## Requirements
An implementation of AI discovery tool that corresponds to AIGuard.Broker and a repository that corresponds to AIGuard.IRepository.

## Architecture
### System Context
![System Context](diagrams/systemcontext.svg)

### Container 
![Container](diagrams/container.svg)

### Component 
![Component](diagrams/ComponentOrchestrator.svg)

## Installation
Configuration parmeters:
```
AIEndpoint : The URL of the AI discovery tool.
PublisherName : The client name connecting to the repository
TopicParser : A regex that determines the topic as derived from the image file name
TopicPosition : The position of the topic as derived from the image file name
QueueName : The repository queue name
WatchFolder : The folder to watch for new files.  These files are passed to the AIEndpoint for discovery
LogFile : The full path and file name where logs are written
WatchedExtensions : The file extension types passed to the AIEndpoint (these file types, if created in the watch folder will be passed to the AIEndpoint)
RepositoryEndpoint : The repository endpoint.
Cameras : A Javascript array containing a list of cameras.
  [ 
    {
      "name": string,   Name of the camera.  The value must be a substring of the image filename.
      "clip": bool,    True will crop the image to the object of interest, false will send the whole image with a target in a bounded rectangle
      "drawTarget": bool,  True will draw a box around the item of interest in the image
      "drawConfidence": bool,  True will print the target and confidence on the image
      "watches": [      A Javascript array containing the items of interest and the confidence level required to produce a match
        {
          "label": string,   The title of an object of interest as returned by the AI
          "confidence": decimal   The confidence level that will produce a match
        }
      ]
    }
  ]
```

## Sample Results Array

[

   {

      "Confidence":0.7237486,

      "Label":"chair",

      "YMin":860,

      "XMin":2141,

      "YMax":1432,

      "XMax":2560

   },

   {

      "Confidence":0.81453925,

      "Label":"potted plant",

      "YMin":535,

      "XMin":1156,

      "YMax":774,

      "XMax":1314

   },

   {

      "Confidence":0.82615024,

      "Label":"chair",

      "YMin":666,

      "XMin":1895,

      "YMax":918,

      "XMax":2088

   },

   {

      "Confidence":0.85697025,

      "Label":"chair",

      "YMin":520,

      "XMin":1643,

      "YMax":789,

      "XMax":1853

   },

   {

      "Confidence":0.8937214,

      "Label":"person",

      "YMin":573,

      "XMin":1881,

      "YMax":1436,

      "XMax":2330

   }

]