<!DOCTYPE html>
<html>
<body>

<?php
	include '../ConfigurationYourSharingEconomyApp.php';
	
	$query_req_consult = "SELECT * FROM proposals WHERE LENGTH(reported) <> 0 AND confirmedreported = 0";
	$result_req_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_req_consult) or die("Query Error::ProposalToxicReport::ReportToxicProposal::Select requests failed");	
	
	echo "<h2>List of reported PROPOSALS</h2><p>";
	
	echo "<table border=\"1\">";
	echo "<tr><td>CREATOR PROFILE</td><td>OFFER TITLE</td><td>OFFER PRICE</td><td>OFFER DESCRIPTION</td><td>DEADLINE</td><td>REPORTER</td><td>LINK TO REQUEST DESCRIPTION</td></tr>";
	while ($row_consult = mysqli_fetch_object($result_req_consult))
	{
		$id_prop = $row_consult->id;
		$user_prop = $row_consult->user;
		$request_prop = $row_consult->request;
		$title_prop = $row_consult->title;
		$description_prop = $row_consult->description;
		$price_prop = $row_consult->price;
		$deadline_prop = $row_consult->deadline;
		$reporter_prop = $row_consult->reported;
		
		$deadline_formatted = date('m/d/Y', $deadline_prop);
		
		// LINK TO THE CREATOR
		$link_creator = "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$user_prop."\">CHECK CREATOR PROFILE</a>";

		// LINK TO THE REPORTER
		$link_reporter = "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$reporter_prop."\">REPORTER PROFILE[$reporter_prop]</a>";		
		
		// LINK TO THE REQUEST
		$linkToCheck = "<a href=\"FormRequests_CheckSingleRequestConsult.php?id=".$request_prop."\">CHECK REQUEST</a>";
		
		$output_main =  "<tr><td>".$link_creator."</td><td>".$title_prop."</td><td>".$price_prop."</td><td>".$description_prop."</td><td>".$deadline_formatted."</td><td>REPORTER[".$link_reporter."]" . "</td><td>".$linkToCheck."</td>";
		
		$output_inform = "";
		$output_inform = $output_inform . "<td>";
		$output_inform = $output_inform . "<form name=\"markConfirmed".$id_prop."\" action=\"FormProposals_ApplyPenaltyToProvider.php\" method=\"post\">";
		$output_inform = $output_inform . "<input type=\"hidden\" name=\"id\" value=\"". $user_prop . "\">";
		$output_inform = $output_inform . "<input type=\"hidden\" name=\"proposal\" value=\"". $id_prop . "\">";			
		$output_inform = $output_inform . "<input type=\"submit\" value=\"CONFIRMATION THAT THE REPORT IS RIGHT\">";
		$output_inform = $output_inform . "</form>";
		$output_inform = $output_inform . "</td>";
		$output_inform = $output_inform . "<td>";
		$output_inform = $output_inform . "<form name=\"markRestored".$id_prop."\" action=\"FormProposals_ApplyPenaltyToConsumer.php\" method=\"post\">";
		$output_inform = $output_inform . "<input type=\"hidden\" name=\"id\" value=\"". $reporter_prop . "\">";
		$output_inform = $output_inform . "<input type=\"hidden\" name=\"proposal\" value=\"". $id_prop . "\">";		
		$output_inform = $output_inform . "<input type=\"submit\" value=\"RESTORE AND PENALTY TO THE REPORTER FOR FALSE ACCUSATION\">";
		$output_inform = $output_inform . "</td>";
		
		$output_main =  $output_main . $output_inform . "</tr>";
		
		echo $output_main;
	}
	 echo "</table>";
	 
	 mysqli_free_result($result_req_consult);

	// Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
?>

</body>
</html>