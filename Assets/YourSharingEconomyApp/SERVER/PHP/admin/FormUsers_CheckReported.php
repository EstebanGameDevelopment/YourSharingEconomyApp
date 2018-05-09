<!DOCTYPE html>
<html>
<body>

<?php
	include '../ConfigurationYourSharingEconomyApp.php';
	
	$query_req_consult = "SELECT requests.id,requests.customer,requests.title,requests.creationdate FROM requests,users WHERE (LENGTH(requests.reported) <> 0 OR requests.flaged = 1) AND ((requests.customer = users.id) AND (users.banned = 0)) ORDER BY requests.flaged DESC, LENGTH(requests.reported) DESC, requests.creationdate DESC, requests.customer DESC";
	$result_req_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_req_consult) or die("Query Error::FormUsers_CheckReported::Select requests failed");

	echo "<h2>List of recent reported USERS by number of times</h2><p>";
	
	echo "<table border=\"1\">";
	echo "<tr><td>USER PROFILE</td><td>TITLE REQUEST</td><td>CREATION DATE</td><td>LINK REQUEST</td></tr>";
	while ($row_req_consult = mysqli_fetch_object($result_req_consult))
	{
		$id_req_reported = $row_req_consult->id;
		$customer_req = $row_req_consult->customer;
		$title_req = $row_req_consult->title;
		$creationdate_req = $row_req_consult->creationdate;
		
		$creationdate_formatted = date('m/d/Y', $creationdate_req);

		// LINK TO THE USER REPORTED
		$link_reported_user = "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$customer_req."\">REPORTED PROFILE[++++$customer_req++++]</a>";		
		
		// LINK TO THE REQUEST REPORTED
		$link_reported_request = "<a href=\"FormRequests_CheckSingleRequestConsult.php?id=".$id_req_reported."\">REPORTER REQUEST[$id_req_reported]</a>";		
		
		echo "<tr><td>".$link_reported_user."</td><td>".$title_req."</td><td>".$creationdate_formatted."</td><td>".$link_reported_request."</td></tr>";
	}
	echo "</table>";
	 
	 mysqli_free_result($result_req_consult);

	// Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
?>

</body>
</html>