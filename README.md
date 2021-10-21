My little .NET 5 pet project that implements notes via a bot in a telegram. Aimed at learning MongoDb technologies, as well as experience in building clean Architecture applications with domain level design in DDD style.
Can be used as a template for building new services. 
Also used docker for mongoDb deployment and app.

The practical benefit of the project lies in the fact that I realized that in the process of learning / working I need something at hand, where I could write my observations or repeat the material covered. The mobile application is not suitable for this, because I needed the ability to reproduce all my notes on a PC and upload them to a doc document. In the future, I plan to expand the functionality and add reminders there, like an alarm clock, only with functionality that I personally need

The bot itself has commands:
1 "/ get ~ theme ~" gets a note on a topic
2 "/ post ~ theme ~ your text" adds / updates a note. To add a picture, you need to throw the same command into the caption.
3 "/ getallthemes" - get all existing note themes

@DotNetTheory_bot 
