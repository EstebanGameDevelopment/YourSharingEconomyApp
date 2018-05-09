<!DOCTYPE html>
<html>
<body>

<?php
	include '../ConfigurationYourSharingEconomyApp.php';

	$id_request = $_GET["id"];
	
	$output_inform = "";
	$output_inform = $output_inform . "<h2>REQUEST CONSULT</h2><p>";	

	$query_consult = "SELECT * FROM requests WHERE id = ". $id_request;
	$result_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consult) or die("Query Error::RequestConsutlByUser::Select requests failed::query_consult=".$query_consult);

	if ($row_consult = mysqli_fetch_object($result_consult))
	{
		$id_req = $row_consult->id;
		$customer_req = $row_consult->customer;
		$providerr_req = $row_consult->provider;
		$title_req = $row_consult->title;
		$description_req = $row_consult->description;
		$price_req = $row_consult->price;
		$currency_req = $row_consult->currency;
		$deadline_req = $row_consult->deadline;
		$deadline_formatted = date('m/d/Y', $deadline_req);
		$deliverydate_req = $row_consult->deliverydate;
		
		if ($deliverydate_req != -1)
		{
			$output_inform = $output_inform . "<hr>";
			
			$output_inform = $output_inform . "<h2>JOB FINISHED</h3><p>";
			
			$output_inform = $output_inform . "<h3>";
			$output_inform = $output_inform . "<table border=\"1\">";			

			// LINK TO THE CUSTOMER
			$link_customerr_creator = "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$customer_req."\">CHECK CUSTOMER PROFILE</a>";

			// LINK TO THE PROVIDER
			$link_providerr_creator = "<a href=\"FormUsers_CheckSingleUserConsult.php?id=".$providerr_req."\">CHECK PROVIDER PROFILE</a>";
			
			$output_inform = $output_inform . "<tr><<td>CUSTOMER</td><td>$link_customerr_creator</td></tr>";
			$output_inform = $output_inform . "<tr><<td>SCORE FROM CUSTOMER TO PROVIDER</td><td>$row_consult->scorecustomer</td></tr>";
			$output_inform = $output_inform . "<tr><td>FEEDBACK FROM CUSTOMER TO PROVIDER</td><td>$row_consult->feedbackcustomer</td></tr>";
			$output_inform = $output_inform . "<tr><<td>PROVIDER</td><td>$link_providerr_creator</td></tr>";
			$output_inform = $output_inform . "<tr><td>SCORE FROM PROVIDER TO CUSTOMER</td><td>$row_consult->scoreprovider</td></tr>";
			$output_inform = $output_inform . "<tr><td>FEEDBACK FROM PROVIDER TO CUSTOMER</td><td>$row_consult->feedbackprovider</td></tr>";
			$output_inform = $output_inform . "</h3>";
			$output_inform = $output_inform . "</table>";		
		}

		$output_inform = $output_inform . "<hr>";
		
		// GET TEXT POST REPORTED		
		$output_inform = $output_inform . "<h3>OFFERS FOR THIS REQUEST</h3><p>";
		$output_inform = $output_inform . "<table border=\"1\">";				
		$query_prop_consult = "SELECT * FROM proposals WHERE request = $id_request";
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
		
		$output_inform = $output_inform . "<h3>THE REQUEST DESCRIPTION</h3><p>";
		$output_inform = $output_inform . "<table border=\"1\">";
		$output_inform = $output_inform . "<tr><td>ID REQUEST</td><td>".$id_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>CUSTOMER ID</td><td>".$customer_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>TITLE</td><td>".$title_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>DESCRIPTION</td><td>".$description_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>PRICE</td><td>".$price_req." ".$currency_req."</td></tr>";
		$output_inform = $output_inform . "<tr><td>DEADLINE</td><td>".$deadline_formatted."</td></tr>";
		
		// GET LINKED IMAGES
		$table_to_check = "requests";
		$query_imgs_consult = "SELECT data,url FROM images WHERE idorigin = $id_request AND tabla='$table_to_check' AND type = 0";
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
		mysqli_free_result($result_imgs_consult);
		$output_inform = $output_inform . "<p><p>";		
		
		$output_inform = $output_inform . "<hr>";		

		// GET LINKED IMAGES
		$output_inform = $output_inform . "<h3>THE FINISHED IMAGES</h3><p>";
		$output_inform = $output_inform . "<table border=\"1\">";
		
		$table_to_check = "requests";
		$query_imgs2_consult = "SELECT data,url FROM images WHERE idorigin = $id_request AND tabla='$table_to_check' AND type <> 0";
		$result_imgs2_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_imgs2_consult) or die("Query Error::RequestConsult::Select images");
	
		while ($row_imgs2_consult = mysqli_fetch_object($result_imgs2_consult))
		{
			$data_img = $row_imgs2_consult->data;
			$url_img = $row_imgs2_consult->url;
			
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
		mysqli_free_result($result_imgs2_consult);
	}
	
	
	
	echo $output_inform;
	 
	 
	mysqli_free_result($result_consult);
	

	// Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
?>

</body>
</html>