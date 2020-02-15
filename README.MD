# AiGateKeeper

The goal of this library is to design a tool that would watch a folder for specific files, send them to an AI dicovery tool and take some action if an appropriate subject is found.
The library has abstracted the AI dicovery tool and the repository.  There are implementations for [DeepStack](https://deepstack.cc/)  and [MQTT](https://github.com/chkr1011/MQTTnet).

## Requirements
An implementation of AI discovery tool that corresponds to AIGaurd.Broker and a repository that corresponds to AIGaurd.IRepository.

## Installation
appsettings configuration:
'''
AIEndpoint : the URL of the AI discovery tool.
'''