Shopping List v1

It holds to separated lists of scrolls, which you want to buy or want to sell. Scrolls 
you want to buy go to your buy list and the scrolls you want to sell to your sell list.

Commands:					

* Add scrolls to your buylist:
	/buy scroll (price) (, anotherScroll (price))*
	Example:	/buy Speed
		/buy Speed, Pother 400, Great Wolf 950

* Add scrolls to your selllist:
	/sell scroll (,anotherScroll)*
	Example:	/sell Speed
		/sell Speed, Pother 300, Great Wolf 800

* Remove scrolls from your buylist:
	/!buy scroll (,anotherScroll)*
	Example:	/!buy Speed
		/!buy Speed, Pother, Great Wolf

* Remove scrolls from your selllist:
	/!sell scroll (,anotherScroll)*
	Example:	/!sell Speed
		/!sell Speed, Pother, Great Wolf

* Remove all scrolls from your buylist:
	/!buy *

* Remove all scrolls from your selllist:
	/!sell *

* Save your lists:
	/export

* Load your lists:
	/import
			
* Print formatted WTB and/or WTS messages:
	/print
	/print buy
	/print sell

* Alter price for your scrolls in any of your lists
	/price scrollname (+|-)? amount
	Example:	/price speed 1000 (speed costs now 1000g)
		/price speed +100 (add 100g to the current price of speed)
		/price speed -100 (subtract 100g from the current price of speed)
