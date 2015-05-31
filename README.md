# GenericToolkit

This started as an exercise in code generation and meta-programming but this solution will hopefully evolve into a way of quickly and easily creating a CRUD API based on Interfaces only. This is a work in progress and therefore will be quite rough around the edges. It's intended for use at hackdays or proof of concepts where an API is needed quickly without having to build all the controllers and logic for CRUD.

The idea of using only Interfaces is to help stick to SOLID principles. The idea being that the majority of your API is based on Interfaces for your Entities but where needed concrete classes can be substituted in. I've not quite worked out how I want to do that yet though ...

## How it works

Take a look at the WebApiExample project you'll see some interfaces in the "Model" folder. There is a naming convention for these which is: 

- 4 interfaces representing the Entity and the HTTP Get/Post/Put operations (Although it'll wire up your IEntity interfaces as a Dto if you forget!)
- As entities are interfaces they should be named "I*xxx*" where "*xxx*" is the name if the entity. The entity interfaces must inherit from IEntity
- All Get/Post/Put in the example should follow the naming convention "I*xxx***Verb**Dto" where "*xxx*" is the name of the entity, "**Verb**" is the http verb capitalised and postfixed with "Dto" (although the postfix can be overridden)
- Delete is done via the Id of your entity and doesn't require a Dto

The configuration stuff is done in Global.asax.cs which is basically generating the controller types and some concrete types based on your interfaces (there's a lot of Reflection.Emit here) and hooking them into the MVC pipeline. In a Web API project all you have to do is add *BootStrapper.RegisterControllers();* to your Global.asax.cs file and set a "GenericContext" connection string to get started!

The controllers are created using the interface name without the "I". E.g. IPerson becomes a PersonController which is accessible in the example through the url */api/person* 
