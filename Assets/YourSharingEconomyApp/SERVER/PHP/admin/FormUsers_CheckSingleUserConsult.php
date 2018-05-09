<!DOCTYPE html>
<html>
<body>

<?php
	include '../ConfigurationYourSharingEconomyApp.php';

	$id_user = $_GET["id"];
	
	$output_inform = $output_inform . "<h2>USER CONSULT</h2><p>";	

	$output_inform = $output_inform . "<hr>";	
	
	$output_inform = $output_inform . "<p><p>";
	$output_inform = $output_inform . "<form name=\"markBanThisUser\" action=\"FormUsers_BanProfile.php\" method=\"post\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"id\" value=\"". $id_user . "\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"ban\" value=\"". 1 . "\">";
	$output_inform = $output_inform . "<input type=\"submit\" value=\"--BAN THIS USER FROM THE SYSTEM--\">";
	$output_inform = $output_inform . "</form>";
	$output_inform = $output_inform . "<p><p>";
	$output_inform = $output_inform . "<p><p>";
	$output_inform = $output_inform . "<form name=\"restoreValidThisUser\" action=\"FormUsers_BanProfile.php\" method=\"post\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"id\" value=\"". $id_user . "\">";
	$output_inform = $output_inform . "<input type=\"hidden\" name=\"ban\" value=\"". 0 . "\">";
	$output_inform = $output_inform . "<input type=\"submit\" value=\"++REMOVE THE BAN APPLIED TO THIS USER++\">";
	$output_inform = $output_inform . "</form>";
	$output_inform = $output_inform . "<p><p>";
	
	$output_inform = $output_inform . "<hr>";	

	// CONSULT USER DATA
	$query_user_consult = "SELECT * FROM users WHERE id = ". $id_user;
	$result_user_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_user_consult) or die("Query Error::RequestConsutlByUser::Select user failed");

	if ($row_usr_consult = mysqli_fetch_object($result_user_consult))
	{
		$name_user = $row_usr_consult->name;
		$description_user = $row_usr_consult->description;
		$ipaddress_user = $row_usr_consult->ipaddress;
		$email_decrypted_user = DecryptText($row_usr_consult->email);
		$facebook_user = $row_usr_consult->facebook;

		$output_inform = $output_inform . "<h3>Consider the next data if you need to report a profile to the local authorities because the nature of the content the user has posted</h3><p>";	
		
		$output_inform = $output_inform . "<h1><table border=\"1\">";
		$output_inform = $output_inform . "<tr><td>USERNAME: ". $name_user . "</td></tr>";	
		$output_inform = $output_inform . "<tr><td>IP ADDRESS: ". $ipaddress_user . "</td></tr>";	
		$output_inform = $output_inform . "<tr><td>EMAIL ADDRESS: ". $email_decrypted_user . "</td></tr>";	
		$linkToFacebookAccount = "<a href=\"https://www.facebook.com/".$facebook_user."\">CHECK FACEBOOK[".$facebook_user."]</a>";
		$output_inform = $output_inform . "<tr><td>FACEBOOK ID: ". $linkToFacebookAccount . "</td></tr>";	
		$output_inform = $output_inform . "</table></h1><p>";
	
		$output_inform = $output_inform . "<hr>";	
	
		$rentstart_user = $row_usr_consult->rentstart;
		$rentdays_user = $row_usr_consult->rentdays;
		
		$scoreuser_user = $row_usr_consult->scoreuser;
		$votesuser_user = $row_usr_consult->votesuser;
		
		$scoreprovider_user = $row_usr_consult->scoreprovider;
		$votesprovider_user = $row_usr_consult->votesprovider;
		
		// IS CONSUMER
		$output_inform = $output_inform . "<h2>REQUESTS AS CONSUMER</h2><p>";	
		$output_inform = $output_inform . "<h2>SCORE CONSUMER=". ((($scoreuser_user * 100)/$votesuser_user))/5 ."%</h2><p>";	

		$query_consumer_consult = "SELECT * FROM requests WHERE customer = ". $id_user;
		$result_consumer_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_consumer_consult) or die("Query Error::RequestConsutlByUser::Select requests consumer failed");

		$output_inform = $output_inform . "<table border=\"1\">";
		while ($row_consumer_consult = mysqli_fetch_object($result_consumer_consult))
		{
			$id_consumer_req = $row_consumer_consult->id;
			$title_consumer_req = $row_consumer_consult->title;
			$price_consumer_req = $row_consumer_consult->price;
			$village_consumer_req = $row_consumer_consult->village;
			$deadline_consumer_req = $row_consumer_consult->deadline;
			$reported_consumer_req = $row_consumer_consult->reported;
			$deliverydate_consumer_req = $row_consumer_consult->deliverydate;
			$flaged_consumer_req = $row_consumer_consult->flaged;
			$confirmflag_consumer_req = $row_consumer_consult->confirmflag;
			
			$array_consumer_reporters = explode(',',$reported_consumer_req);
			$deadline_consumer_formatted = date('m/d/Y', $deadline_consumer_req);
			$number_consumer_reporters = sizeof($array_consumer_reporters);
			
			$work_is_finished = "FINISHED WORK!!";
			if ($deliverydate_consumer_req == -1)
			{
				$work_is_finished = "NOT FINISHED";	
				if (($flaged_consumer_req == 1) || ($confirmflag_consumer_req == 1))
				{
					$work_is_finished = "TOXIC FLAG!!!";		
				}
			}
			
			$linkRequestConsumerToCheck = "<a href=\"FormRequests_CheckSingleRequestConsult.php?id=".$id_consumer_req."\">CHECK REQUEST</a>";
			$output_inform = $output_inform . "<tr><td>".$title_consumer_req."</td><td>".$price_consumer_req."</td><td>".$village_consumer_req."</td><td>".$deadline_consumer_formatted."</td><td>REPORTERS[".$number_consumer_reporters."]:".$reported_consumer_req."</td><td>".$linkRequestConsumerToCheck."</td><td>".$work_is_finished."</td></tr>";
		}
		 $output_inform = $output_inform . "</table>";
		 mysqli_free_result($result_consumer_consult);

		 $output_inform = $output_inform . "<p><p>";

		 $output_inform = $output_inform . "<hr>";	
		 
		// IS PROVIDER
		if ($rentstart_user != -1)
		{
			$output_inform = $output_inform . "<h2>REQUESTS WORKED AS PROVIDER</h2><p>";	
			$output_inform = $output_inform . "<h2>SCORE PROVIDER=". ((($scoreprovider_user * 100)/$votesprovider_user)/5) ."%</h2><p>";	
			$output_inform = $output_inform . "<h2>DESCRIPTION</h2><p>";
			$output_inform = $output_inform . "<h3>" . $description_user . "</h3>";
			$output_inform = $output_inform . "<p><p>";
			
			$query_provider_consult = "SELECT * FROM requests WHERE provider = ". $id_user;
			$result_provider_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_provider_consult) or die("Query Error::RequestConsutlByUser::Select requests consumer failed");

			$output_inform = $output_inform . "<table border=\"1\">";
			while ($row_provider_consult = mysqli_fetch_object($result_provider_consult))
			{
				$id_provider_req = $row_provider_consult->id;
				$title_provider_req = $row_provider_consult->title;
				$price_provider_req = $row_provider_consult->price;
				$village_provider_req = $row_provider_consult->village;
				$deadline_provider_req = $row_provider_consult->deadline;
				$reported_provider_req = $row_provider_consult->reported;
				$deliverydate_provider_req = $row_provider_consult->deliverydate;
				$flaged_provider_req = $row_provider_consult->flaged;
				$confirmflag_provider_req = $row_provider_consult->confirmflag;

				$array_provider_reporters = explode(',',$reported_provider_req);
				$deadline_provider_formatted = date('m/d/Y', $deadline_provider_req);
				$number_provider_reporters = sizeof($array_provider_reporters);
				
				$work_is_finished = "FINISHED WORK!!";
				if ($deliverydate_provider_req == -1)
				{
					$work_is_finished = "NOT FINISHED";	
					if (($flaged_consumer_req == 1) || ($confirmflag_consumer_req == 1))
					{
						$work_is_finished = "TOXIC FLAG!!!";		
					}					
				}
				
				$linkRequestConsumerToCheck = "<a href=\"FormRequests_CheckSingleRequestConsult.php?id=".$id_provider_req."\">CHECK REQUEST</a>";
				$output_inform = $output_inform . "<tr><td>".$title_provider_req."</td><td>".$price_provider_req."</td><td>".$village_provider_req."</td><td>".$deadline_provider_formatted."</td><td>REPORTERS[".$number_provider_reporters."]:".$reported_provider_req."</td><td>".$linkRequestConsumerToCheck."</td><td>".$work_is_finished."</td></tr>";
			}
			 $output_inform = $output_inform . "</table>";
			 mysqli_free_result($result_provider_consult);		

			 $output_inform = $output_inform . "<p><p>";
			 
			 $output_inform = $output_inform . "<hr>";	
			 
			 // IMAGES PROVIDER
			 $output_inform = $output_inform . "<h2>IMAGES EXPERIENCE PROVIDER</h2><p>";	
			 
			// GET LINKED IMAGES
			$table_to_check = "users";
			$query_imgs_consult = "SELECT data,url FROM images WHERE idorigin = $id_user AND tabla='$table_to_check'";
			$result_imgs_consult = mysqli_query($GLOBALS['LINK_DATABASE'],$query_imgs_consult) or die("Query Error::RequestConsult::Select images");
		
			$output_inform = $output_inform . "<table border=\"1\">";
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
		}
	}
	
	echo $output_inform;
	 
	mysqli_free_result($result_user_consult);
	
	// Closing connection
    mysqli_close($GLOBALS['LINK_DATABASE']);
?>

</body>
</html>