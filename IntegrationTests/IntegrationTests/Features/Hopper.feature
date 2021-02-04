Feature: Hopper
	

Background:
	Given a creative superflat world on level '4'

Scenario: Transfer item from one chest to another
	
	Given there is the 'Chest#1' at X:0,Z:0
	And there is the 'Hopper#1' at X:1,Z:0 pointing to 'Chest#1'
	And there is the 'Chest#2' at X:1,Y:5,Z:0

	When a player puts an item into 'Chest#2'

	Then it appears in 'Chest#1'