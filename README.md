# GenericToolkit

An exercise in code generation and meta-programming. This solution will hopefully evolve into a way of quickly and easily creating a CRUD API based on Interfaces only. This is a work in progress and therefore will be quite rough around the edges. It's intended for use at hackdays or proof of concepts where an API is needed quickly without having to build all the controllers and logic for CRUD.

The idea of using only Interfaces is to help stick to SOLID principles. The idea being that the majority of your API is based on Interfaces for your Entities but where needed concrete classes can be substituted in. I've not quite worked out how I want to do that yet though ...
