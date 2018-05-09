<!DOCTYPE html>
<html>
<body>

<?php
	include '../ConfigurationYourSharingEconomyApp.php';

	
	$query_consult = "SELECT * FROM requests WHERE confirmflag = 0 AND (LENGTH(reported) <> 0 OR flaged = 1) ORDER BY flaged DESC, LENGTH(reported) DESC";
	$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestConsutlByUser::Select requests failed");
	
	echo "<h2>List of reported requests</h2><p>";
	
	echo "<table border=\"1\">";
	echo "<tr><td>CREATOR PROFILE</td><td>REQUEST TITLE</td><td>REQUEST PRICE</td><td>VILLAGE</td><td>DEADLINE</td><td>REPORTERS</td><td>LINK TO REQUEST DESCRIPTION</td></tr>";
	while ($row_consult = mysqli_fetch_object($result_consult))
	{
		$id_req = $row_consult->id;
		$customer_req = $row_consult->customer;
		$title_req = $row_consult->title;
		$price_req = $row_consult->price;
		$village_req = $row_consult->village;
		$deadline_req = $row_consult->deadline;
		$reported_req = $row_consult->reported;
		
		$array_reporters = explode(',',$reported_req);
		$deadline_formatted = date('m/d/Y', $deadline_req);
		$number_reporters = sizeof($array_reporters);

		// LINKS TO THE REPORTERS
		$links_reporters = "";
		foreach ($array_reporters as &$bad_reporter)
		{
			$links_reporters = $links_reporters . "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$bad_reporter."\">REPORTER PROFILE[$bad_reporter]</a>";
			$links_reporters = $links_reporters . ",";
		}
		
		// LINK TO THE CREATOR
		$link_creator = "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$customer_req."\">CHECK CREATOR PROFILE</a>";
		
		// LINK TO THE REQUEST
		$linkToCheck = "<a href=\"FormRequests_CheckSingleRequestReport.php?id=".$id_req."&user=".$customer_req."&reporters=".$reported_req."\">CHECK REQUEST</a>";
		
		// ECHO THE NEW REPORTED REQUEST SUMMARY
		echo "<tr><td>".$link_creator."</td><td>".$title_req."</td><td>".$price_req."</td><td>".$village_req."</td><td>".$deadline_formatted."</td><td>$number_reporters REPORTERS:".$links_reporters."</td><td>".$linkToCheck."</td></tr>";
	}
	 echo "</table>";
	 
	 mysqli_free_result($result_consult);

	// Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
?>

</body>
</html>