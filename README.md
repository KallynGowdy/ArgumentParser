#Argument Parser
An easy-to-use command-line argument parser that allows you to provide advanced configurations in one statement.

##How to use

You can define an entire parser with one statement in C# because it is all contained in the initialization of the parser object. In your console application you have a `Main` method which can receive an array of strings as arguments. You provide these arguments to the parser that you define to enable and disable features. Features can be enabled and disabled while the parser is running because it uses delegates (Specifically predicates) as callback devices.

These are the steps to making a complete parser:

1. Define the parser
2. Run the parser
3. Done!

###Step 1
Define the parser

You define a parser using object declaration statements. For example, we can define an argument parser that takes one positional argument like so:

	using KallynGowdy.ArgumentParser;

	public class Program
	{
		static ArgumentParser parser = new ArgumentParser
    	{
    	    Arguments = new IArgument[]
    	    {
    	        //A positional argument is passed based on it's position relative to the first place,
    	        //This example accepts one argument at the beginning of the argument list that is supposed to be the user's name. 
    	        //(i.e. The command to execute this program is 'ProgramName.exe <YourName>')
    	        new PositionalArgument<string>
    	        {
    	            //The zero-based position of where the argument should be relative to other positional arguments.
    	            Position = 0,

    	            //The text that is displayed for this argument as a suplement to the ValueName. Used to provide more context/detail about the value required.
    	            HelpText = "Your name",

    	            //The default value that is passed to OnMatch if a value is not provided. Not used since the arguement is required.
    	            DefaultValue = null,

    	            //Whether this argument is required.
    	            Required = true,

    	            //The placeholder value that is the name of the value that should be provided
    	            ValueName = "Name",
	
    	            //OnMatch is a Predicate, so we need to return whether the given value is valid
    	            OnMatch = givenString =>
    	            {
    	                if(givenString != null)
    	                {
    	                    //Set the name that we should display to the provided value and return that the value was valid.
    	                    personName = givenString;
	
    	                    return true;
    	                }
    	                //return that the provided value was invalid and that we should display an error along with the help text.
    	                return false;
    	            }
    	        }
    	    }
    	};

		//The name of the person that we should say "Hi!" to.
		static string personName;
	}
Technically, that declaration is only **one** statement. By using many more arguments you can quickly and easily add features to your console application. You can also define **Named Arguments**, named arguments are usually easier to provide than positional arguments because they provide more clarity. The `ping` command on Windows takes named arguments first and then positional arguments. Some of the named arguments are using `-l` to accept packet sizes, otherwise the default is 32 bytes. You can provide these same features easily with the ArgumentParser.

    //A named argument is passed using "names" for it. These "names" are just simple markers for values.
    //For example, this Named Argument accepts a string as a value, that value can be passed by placing either
    //of the definitions('-g' or '--greeting') in front of the value that you want to pass, like this:
    //"-g Hello" (to pass the value "Hello")
    new NamedArgument<string>
    {
        //The definitions are all of the posible aliases that could refer to this argument.
        Definitions = new string[]
        {
            "-g",
            "--greeting"
        },
    
        //The default value is the value that should be given if this argument is not provided.
        DefaultValue = "Hello",
    
        //HasValue determines if this Named Argument accepts a value, 'true' signifies that a value can be attached right after it as an
        //arbitrary parameter. 'false' signifies that the PassedValue should be used.
        HasValue = true,
    
        //PassedValue is the value that should be given if this argument is provided without a value. That is, without an arbitrary parameter right after it.
        PassedValue = "Hello",
    
        //The HelpText is the text that should be shown as an explanation for what this argument does.
        HelpText = "The greeting to extend toward the user",
    
        //Whether this argument is required to be passed by the user.
        Required = false,
    
        //The user-friendly name of the value that should be passed by the user.
        ValueName = "Greeting",
    
        OnMatch = givenString =>
            {
                if(givenString != null)
                {
                    greeting = givenString;
                    return true;
                }
                return false;
            }
    }

###Step 2
Run the parser

So, using the parser that we just defined, we can now supply the command line arguments to the parser and let it do it's work.

	static void Main(string[] args)
	{
		//GetValues also returns a Dictionary that relates the argument objects that you created above
		//to the values that they contain, however since we can use OnMatch to set the values that
		//we need, we don't need to fiddle with any fancy dictionary work. If GetValues
		//Returns null, then a required argument was not passed.
		if(parser.GetValues(args) != null)
		{
			//Print our "Hello <Person>!" text
			Console.WriteLine("Hello {0}!", personName);
		}
	}