MyCFBD is a college football data entry app. My grandfather was incredibly passionate about college football, and would spend the whole season manually writing data from every game on printed-out spreadsheets for his records. There are existing college football databases, but many don't include historical betting data, and many have paywall-locked features. Since I already have all the data for over a decade, I made this app to streamline the data entry process so I can build by own database for data analysis and visualization instead of paying for access to someone else's data.

To build and run the application:
1. In a terminal or GitBash, navigate to the project's root directory and run <b>dotnet build</b>.
2. Navigate to the sub-directory /MyCFBD.API and run <b>dotnet run</b>. You can then navigate to the port localhost:7069/scalar/v1 to test the API calls the data entry app uses, or see the JSON structure.
3. Navigate to ../DataEntryApp and run <b>dotnet run</b>. Follow the instructions (PrintAll, PrintByID, etc.) to add and view teams. Type Stop when you're done adding.

You can also test the application in /MyCFBD.Tests by running <b>dotnet test</b>

<i>What I learned</i>

I came into this course with a lot of experience in Python and GDScript (which is very similar to Python) but no other languages. Learning C# has given me better habits when I do use Python, like type hinting to make my projects' intended data types clearer. Another habit the course has taught me is that instead of trial-and-error debugging or adding print statements until I find the error, I'll actually use the debugger and breakpoints to see where things go off track. 

<i>What I'd have done differently</i>

If I had more time, I would have added two main things: A way to add game results for teams instead of just basic team data, and after that more ways to sort the data (for example, sorting by teams that covered the spread the most often). Ultimately, though, I think the best approach is to use this app strictly for data entry and do the actual analysis/visualization in Python with Pandas/Matplotlib like I tend to.
