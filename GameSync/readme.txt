options:
	integrate with dropbox via hard links
	+ easy, no need to daemonize

user story: 
	user launches program
objective:
	ensure hard links are configured properly
After program launches, program should daemonize and poll missing folders for content 
every x seconds (probably large, like 60 minutes). This should also be delayed on 
startup (probably 10 minutes or so).

user story:
	user shows gui
objective:
	show user a list of all available applications, show which apps are active.