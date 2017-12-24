[![Build Status](https://travis-ci.org/dillonwd6086/DotnetCoin.svg?branch=master)](https://travis-ci.org/dillonwd6086/DotnetCoin)
# DotnetCoin
The purpose of this repository is to demonstrate a Crypto-Currency implementation using DotnetCore for educational purposes.

`Currently Works on 1 node`

## Structure

There are several main components to this project.

1.  A Class library
  - Contains all of the object necessary for creating a Crypto-Currency on a single node.
  - Includes a Node object which is the main interface for spawning up a Crypto-Currency node.
  - ProofOfWork object which controls the difficulty of the chain

2.  A `Restful` webapi
  - Provides the ability for different nodes to interact with one another
  - Hosts a website that
    - Displays all the blocks in the chain
    - Displays any pending transactions
    - Allows the creation of new transactions
    - Initiates the mining of pending transactions
    - Displays the balance for a user

3.  A Testing Project for the Class Library

4.  A Console application which allows the use of the webapi.

## Running Locally

Inorder to run the site locally clone the repo and navigate to DotcoinApi then run the following commands:

```
dotnet build
dotnet run
```

Then navigate to `http://localhost:5000/BlockChain.html`

## Roadmap

1.  Get working on a single node - In Progress
2.  Begin adding logic to work across multiple nodes
3.  Deploy across multiple nodes
4.  Expand proof of work class to have a more difficult mining function as well as create a set of rules to increase difficulty over time.
