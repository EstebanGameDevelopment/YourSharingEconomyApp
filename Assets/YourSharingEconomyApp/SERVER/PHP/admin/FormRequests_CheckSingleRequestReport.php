<!DOCTYPE html>
<html>
<body>

<?php
	include '../ConfigurationYourSharingEconomyApp.php';

	$id_request = $_GET["id"];
	$user_request = $_GET["user"];
	$reporters_request = $_GET["reporters"];
	
	$output_inform = "";
	$output_inform = $output_inform . "<h2>Reported Request</h2><p>";	

	$output_inform = $output_inform . "<hr>";	
	
	$output_inform = $output_inform . "<p><p>";
	$output_inform = $output_inform . "<form name=\"markConfirmed\" action=\"FormRequests_MarkedFlaggedConfirmation.php\" method=\"post\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"id\" value=\"". $id_request . "\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"user\" value=\"". $user_request . "\">";
	$output_inform = $output_inform . "<input type=\"submit\" value=\"MARK REQUEST AS CONFIRMED FLAGGED\">";
	$output_inform = $output_inform . "</form>";
	$output_inform = $output_inform . "<p><p>";
	$output_inform = $output_inform . "<form name=\"markRestored\" action=\"FormRequests_RestoreRequestPenaltyForReporters.php\" method=\"post\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"id\" value=\"". $id_request . "\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"reporters\" value=\"". $reporters_request . "\">";
	$output_inform = $output_inform . "<input type=\"submit\" value=\"RESTORE REQUEST AS VALID AND APPLY A PENALTY OVER THE REPORTERS\">";
	$output_inform = $output_inform . "<p><p>";
	$output_inform = $output_inform . "</form>";

	$output_inform = $output_inform . "<hr>";	
	
	$query_consult = "SELECT * FROM requests WHERE id = ". $id_request;
	$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestConsutlByUser::Select requests failed");

	if ($row_consult = mysqli_fetch_object($result_consult))
	{
		$id_req = $row_consult->id;
		$customer_req = $row_consult->customer;
		$title_req = $row_consult->title;
		$description_req = $row_consult->description;
		$price_req = $row_consult->price;
		$currency_req = $row_consult->currency;
		$deadline_req = $row_consult->deadline;
		$deadline_formatted = date('m/d/Y', $deadline_req);

		// GET TEXT POST REPORTED		
		$output_inform = $output_inform . "<h3>What reporters are saying about this request</h3><p>";
		$output_inform = $output_inform . "<table border=\"1\">";				
		$query_prop_consult = "SELECT * FROM proposals WHERE request = $id_request AND type = 2";
		$result_prop_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_prop_consult) or die("Query Error::ProposalsResetRequestAll::Select proposals failed");
		
		while ($row_prop_consult = mysqli_fetch_object($result_prop_consult))
		{
			$title_prop = $row_prop_consult->title;
			$description_prop = $row_prop_consult->description;
			$usr_prop_entry = $row_prop_consult->user;
			
			// LINK TO THE PROVIDER OF THE PROPOSAL
			$link_prop_creator = "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$usr_prop_entry."\">CHECK CREATOR PROFILE</a>";
			$output_inform = $output_inform . "<tr><td>".$link_prop_creator."</td><td>".$title_prop."</td><td>".$description_prop."</td></tr>";	
		}
		$output_inform = $output_inform . "</table>";		
		$output_inform = $output_inform . "<p><p>";
		
		$output_inform = $output_inform . "<hr>";	
		
		$output_inform = $output_inform . "<h3>THE CONFLICTIVE REQUEST</h3><p>";
		$output_inform = $output_inform . "<table border=\"1\">";
		$output_inform = $output_inform . "<tr><td>ID REQUEST</td><td>".$id_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>CUSTOMER ID</td><td>".$customer_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>TITLE</td><td>".$title_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>DESCRIPTION</td><td>".$description_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>PRICE</td><td>".$price_req." ".$currency_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>DEADLINE</td><td>".$deadline_formatted."</td></tr>";
		
		// GET LINKED IMAGES
		$table_to_check = "requests";
		$query_imgs_consult = "SELECT data,url FROM images WHERE idorigin = $id_request AND tabla='$table_to_check'";
		$result_imgs_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_imgs_consult) or die("Query Error::RequestConsult::Select images");
	
		while ($row_imgs_consult = mysqli_fetch_object($result_imgs_consult))
		{
			$data_img = $row_imgs_consult->data;
			$url_img = $row_imgs_consult->url;
			
			$img_from_data = base64_encode($data_img);

			$final_image_html = "";
			if (strlen($url_img) > 0)
			{
				$final_image_html = "<a href=\"" . $url_img . "\"><img alt=\"105x105\" class=\"img-responsive\" src=\"data:image/jpg;charset=utf8;base64," . $img_from_data ."\"/></a>";	
			}
			else
			{
				$final_image_html = "<img alt=\"105x105\" class=\"img-responsive\" src=\"data:image/jpg;charset=utf8;base64," . $img_from_data ."\"/>";	
			}
			
			$output_inform = $output_inform . "<tr><td>IMAGE</td><td>".$final_image_html."</td></tr>";
		}	
		$output_inform = $output_inform . "</table>";		
	}
	
	echo $output_inform;
	 
	 
	mysqli_free_result($result_consult);
	

	// Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
?>

</body>
</html>