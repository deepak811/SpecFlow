Feature: Flight Search
	In order to book flight
	As a unregistered User
	I want to search flight with given details


Background: User navigates to Search Page
	Given I am on the HomePage
	When I click 'Flight' option on HomePage
	Then I see Flight Search Page 

@Smoke
Scenario Outline: US1001_AC01_01 - Search One Way Flight
	Given I entered the followings data as Flight Search Criteria
		| From   | To        | FromOffSet | ToOffSet | Adult | Child | Infant | Class   |
		| Mumbai | Amsterdam | 5          | 10       | 3     | 3     | 3      | Economy |     
	And I update the Flight Search Criteria as '<Trip>'
	When I Click 'SearchButton' button on Flight Search Page
	And I see Flight Records displayed are valid as followings
		| From   | To        | FromOffSet | ToOffSet | Adult | Child | Infant | Class   |
		| Mumbai | Amsterdam | 5          | 10       | 3     | 3     | 3      | Economy |
	Then Displayed Flight Records are valid for 'OneWay' 

Examples: 
	| SID            | SD                    | Trip   |
	| US1001_AC01_01 | Search One Way Flight | OneWay |


