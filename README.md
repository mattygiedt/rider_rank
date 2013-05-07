rider_rank
==========

USAC data downloader and experimental BikeReg based rider ranking engine

This project consists of two separate applications:

1. The 'data_downloader' used to scrape the USAC/USA CRITS rider ranking database;
2. The 'rider_ranking' engine used to rank each rider listed in a BikeReg download file.

Most race/technical directors will hopefully find the data download application usefull. The Glencoe Grand Prix (www.glencoegrandprix.com) will be using the rider ranking engine to stage the 2013 race.

Suggested usage is to start both applications from the command line:

1. Please see the data_downloader.exe.config file for example configurations. The current configuration pulls both Men and Women USA CRITS and USAC Road rider points. The mountain and track discipline's are commented out.
2. The rider_rank application requires a BikeReg CSV file as input. Custom configuration of the rider_rank.exe.config will be required to align the ranking engine with your event. Simply match the 'Category Entered/Merchandise Ordered' BikeReg column to a custom event.

To build the applications from source:

1. Start command (cmd) shell and navigate to base rider_rank directory.
2. Type 'build' [enter]
