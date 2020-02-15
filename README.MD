# AIGateKeeper

The goal of this library is to design a tool that would watch a folder for specific files, send them to an AI discovery tool and take some action if an appropriate subject is found.
The library has abstracted the AI discovery tool and the repository.  There are implementations for [DeepStack](https://deepstack.cc/)  and [MQTT](https://github.com/chkr1011/MQTTnet).

## Requirements
An implementation of AI discovery tool that corresponds to AIGaurd.Broker and a repository that corresponds to AIGaurd.IRepository.

## Installation
Configuration parmeters:
```
AIEndpoint : The URL of the AI discovery tool.
PublisherName : The client name connecting to the repository
TopicParser : A regex that determines the topic as derived from the image file name
TopicPosition : The position of the topic as derived from the image file name
QueueName : The repository queue name
WatchFolder : The folder to watch for new files.  These files are passed to the AIEndpoint for discovery
WatchedExtensions : The file extension types passed to the AIEndpoint (these file types, if created in the watch folder will be passed to the AIEndpoint)
RepositoryEndpoint : The repository endpoint.
WatchedObjects : A Javascript object containing the items that trigger a write to the repository
  { 
    "person": "0.75" : Items of interest and the confidence level that trigger a repository write
  }
```