# CodingFox Programming Language

[License](LICENSE)

## What is CodingFox?

CodingFox is my attempt at making a toy programming language based on the info at [Crafting Interpreters](https://craftinginterpreters.com).
It is a C# app that compiles code to byte code, which makes it reasonably fast, but also interpreted.

Keep in mind this is by far not a great example of how to make a coding language, but it is a fun project to work on.

## Features

* Variables and read-only variables
* Type system
* Classes and inheritance
* Properties with optional get/set

## What does it look like?

```swift
let start = clock();

class Food
{
	var _name: string;
	var taste: int32;

	var name: string
	{
		get
		{
			return this._name;
		}

		set
		{
			this._name = value;
		}
	}

	init(name: string)
	{
		this._name = name;

		print "Food Init";
	}

	func toString(): string
	{
		return name + " taste: " + taste + " (" + typeof(this) + ")";
	}
}

class Bacon: Food
{
	init(name: string)
	{
		super.init(name);

		print "Bacon init";
	}
}

print "asdf length: " + "asdf".length;

let b = Bacon("My bacon");

b.taste = 123;

print b;

b.name = "My other bacon";

b.taste = 456;

print b;

print typeof(b);

print (clock() - start) + " ms elapsed";
```

## TO DO
* Cleanup code (it is very sloppy even by my standards)
* LLVM backend to make this properly compiled (and fast!)

Leftover from rewrite for bytecode:
* For/While cycles
* Logical (and/or)
* Ifs
