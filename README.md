# GenericToolkit

An exercise in code generation and meta-programming. This solution will hopefully evolve into a way of quickly and easily creating a CRUD API based on Interfaces only. This is a work in progress and therefore will be quite rough around the edges. It's intended for use at hackdays or proof of concepts where an API is needed quickly without having to build all the controllers and logic for CRUD.

The idea of using only Interfaces is to help stick to SOLID principles. The idea being that the majority of your API is based on Interfaces for your Entities but where needed concrete classes can be substituted in. I've not quite worked out how I want to do that yet though ...

## How it works

Take a look at the WebApiExample project you'll see some interfaces in the "Model" folder. There is a naming convention for these which is: 

- 4 classes representing the Entity and the HTTP Get/Post/Put operations 
- As entities are interfaces they should be named "I*xxx*" where "*xxx*" is the name if the entity 
- All Get/Post/Put in the example should follow the naming convention "I*xxxx***Verb**Dto" where "*xxx*" is the name of the entity, "**Verb**" is the http verb capitalised and postfixed with "Dto" (although the postfix can be overridden)

The configuration stuff is done in Global.asax.cs which is basically generating the controller types (there's a lot of Reflection.Emit here) and hooking them into the MVC pipeline.

The controllers are created using the interface name without the "I". E.g. IPerson becomes a PersonController which is accessible in the example through the url /api/person 
