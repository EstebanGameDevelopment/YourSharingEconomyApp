using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using YourCommonTools;

namespace YourSharingEconomyApp
{

	/******************************************
	 * 
	 * ScreenProposalView
	 * 
	 * It allows the provider to do 3 different operations:
	 * 
	 *      1.The provider can make an offer to the owner of the request to consider to get the job
	 *      2.The provider can ask a question to the owner of the request.
	 *      3.The provider can report the request as inaproppiate
	 * 
	 * @author Esteban Gallardo
	 */
	public class ScreenProposalView : ScreenBaseView, IBasicView
	{
		public const string SCREEN_CREATE_PROPOSAL = "SCREEN_CREATE_PROPOSAL";
		public const string SCREEN_DISPLAY_PROPOSAL = "SCREEN_DISPLAY_PROPOSAL";
		public const string SCREEN_QUESTION_PROPOSAL = "SCREEN_QUESTION_PROPOSAL";

		// ----------------------------------------------
		// SUBS
		// ----------------------------------------------	
		public const string SUB_EVENT_SCREENPROPOSAL_CONFIRMATION = "SUB_EVENT_SCREENPROPOSAL_CONFIRMATION";
		public const string SUB_EVENT_SCREENPROPOSAL_EXIT_WITHOUT_SAVING = "SUB_EVENT_SCREENPROPOSAL_EXIT_WITHOUT_SAVING";
		public const string SUB_EVENT_SCREENPROPOSAL_WANT_TO_REMOVE = "SUB_EVENT_SCREENPROPOSAL_WANT_TO_REMOVE";
		public const string SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_OK = "SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_OK";
		public const string SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_KO = "SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_KO";
		public const string SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_DEAL = "SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_DEAL";
		public const string SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT = "SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT";
		public const string SUB_EVENT_SCREENPROPOSAL_REPORT_TOXIC_OFFER = "SUB_EVENT_SCREENPROPOSAL_REPORT_TOXIC_OFFER";

		// ----------------------------------------------
		// PRIVATE MEMBERS
		// ----------------------------------------------	
		private GameObject m_root;
		private Transform m_container;
		private ProposalModel m_proposalData;
		private RequestModel m_requestData;
		private bool m_isReadyToPublish = false;
		private bool m_hasChanged = false;

		private bool m_isDisplayInfo = false;
		private bool m_hasBeenInited = false;

		private Transform m_offerExclusiveContent;

		private GameObject m_buttonSave;

		private Transform m_buttonReport;

		private GameObject m_buttonAcceptAndKeepLooking = null;
		private GameObject m_buttonCloseDeal = null;

		public bool IsReadyToPublish
		{
			get { return m_isReadyToPublish; }
			set
			{
				if (!m_isDisplayInfo)
				{
					m_hasChanged = true;
					if (m_proposalData.AllDataFilled())
					{
						m_isReadyToPublish = value;
					}
					m_buttonSave.SetActive(m_isReadyToPublish);
				}
			}
		}

		// -------------------------------------------
		/* 
		 * Constructor
		 */
		public override void Initialize(params object[] _list)
		{
			m_root = this.gameObject;
			m_container = m_root.transform.Find("Content");

			m_proposalData = new ProposalModel();
			bool isOnlyQuestion = false;
			if (_list.Length > 0)
			{
				if (_list[0] != null)
				{
					// DISPLAY EXISTING PROPOSAL
					if (_list[0] is ProposalModel)
					{
						m_proposalData = ((ProposalModel)_list[0]).Clone();
						m_requestData = ((RequestModel)_list[1]).Clone();
					}
					// NEW PROPOSAL FOR THE REQUEST ID
					if (_list[0] is RequestModel)
					{
						m_proposalData.User = UsersController.Instance.CurrentUser.Id;
						m_requestData = ((RequestModel)_list[0]);
						m_proposalData.Request = m_requestData.Id;
						m_proposalData.Deadline = m_requestData.Deadline;
						if (_list.Length > 1)
						{
							isOnlyQuestion = true;
						}
					}
				}
			}

			if (m_container.Find("Button_Back/Icon") != null)
			{
				m_isDisplayInfo = true;
			}
			else
			{
				m_isDisplayInfo = false;
			}

			m_container.Find("Button_Back").GetComponent<Button>().onClick.AddListener(BackPressed);
			if (!m_isDisplayInfo)
			{
				m_buttonSave = m_container.Find("Button_Save").gameObject;
				m_buttonSave.GetComponent<Button>().onClick.AddListener(SavePressed);
			}

			IsReadyToPublish = false;

			if (m_container.Find("TypeDropdown") != null)
			{
				m_container.Find("TypeDropdown").GetComponent<Dropdown>().onValueChanged.AddListener(OnTypeMessage);
				m_container.Find("TypeDropdown").GetComponent<Dropdown>().itemText.text = LanguageController.Instance.GetText("screen.proposal.type.option.proposal");
				m_container.Find("TypeDropdown").GetComponent<Dropdown>().options[0].text = LanguageController.Instance.GetText("screen.proposal.type.option.proposal");
				m_container.Find("TypeDropdown").GetComponent<Dropdown>().options[1].text = LanguageController.Instance.GetText("screen.proposal.type.option.information");
				m_container.Find("TypeDropdown").GetComponent<Dropdown>().options[2].text = LanguageController.Instance.GetText("screen.proposal.type.option.report");
				m_container.Find("TypeDropdown").GetComponent<Dropdown>().value = 2;
				if (m_container.Find("TypeLabel") != null)
				{
					m_container.Find("TypeLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.proposal.type.label");
				}
				if (isOnlyQuestion)
				{
					m_proposalData.Type = ProposalModel.TYPE_INFO;
					m_container.Find("TypeDropdown").GetComponent<Dropdown>().interactable = false;
				}
			}

			if (m_isDisplayInfo)
			{
				Transform buttonRemove = m_root.transform.Find("Content/Button_Remove");
				if (buttonRemove != null)
				{
					buttonRemove.gameObject.SetActive(false);
					if ((m_proposalData.User == UsersController.Instance.CurrentUser.Id) && m_proposalData.CanBeRemoved())
					{
						buttonRemove.gameObject.SetActive(true);
						buttonRemove.GetComponent<Button>().onClick.AddListener(RemovePressed);
					}
					if (m_requestData.Provider != -1)
					{
						buttonRemove.gameObject.SetActive(false);
					}
				}

				// BUTTON REACTIVATE OFFER
				Transform buttonReactivateOffer = m_root.transform.Find("Content/Button_Reactivate");
				if (buttonReactivateOffer != null)
				{
					buttonReactivateOffer.gameObject.SetActive(false);
					if (!m_proposalData.IsActiveOffer() &&
						(m_proposalData.User == UsersController.Instance.CurrentUser.Id))
					{
						buttonReactivateOffer.gameObject.SetActive(true);
						buttonReactivateOffer.Find("Text").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.proposal.offer.reactive.it");
						buttonReactivateOffer.GetComponent<Button>().onClick.AddListener(OnClickReactivateOffer);
					}
				}

				m_buttonReport = m_root.transform.Find("Content/Button_Report");
				if (m_buttonReport != null)
				{
					m_buttonReport.gameObject.SetActive(false);
					if ((m_requestData.Customer == UsersController.Instance.CurrentUser.Id) && m_proposalData.CanBeReported() && !m_proposalData.IsSelected())
					{
						m_buttonReport.gameObject.SetActive(true);
						m_buttonReport.GetComponent<Button>().onClick.AddListener(ReportPressed);
					}
					if (m_requestData.Provider != -1)
					{
						m_buttonReport.gameObject.SetActive(false);
					}
				}
			}

			m_container.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.proposal.title");

			if (m_container.Find("TitleLabel") != null)
			{
				m_container.Find("TitleLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.proposal.label.title");
			}
			if (!m_isDisplayInfo)
			{
				m_container.Find("TitleValue").GetComponent<InputField>().text = m_proposalData.Title;
				m_container.Find("TitleValue").GetComponent<InputField>().onEndEdit.AddListener(OnProposalTitle);
			}
			else
			{
				m_container.Find("TitleValue").GetComponent<Text>().text = m_proposalData.Title;
			}

			if (m_container.Find("DescriptionLabel") != null)
			{
				m_container.Find("DescriptionLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.proposal.label.description");
			}
			if (!m_isDisplayInfo)
			{
				m_container.Find("DescriptionValue").GetComponent<InputField>().text = m_proposalData.Description;
				m_container.Find("DescriptionValue").GetComponent<InputField>().onEndEdit.AddListener(OnProposalDescription);
			}
			else
			{
				m_container.Find("ScrollDescriptionValue/DescriptionValue").GetComponent<Text>().text = m_proposalData.Description;
			}

			// BUTTON CHECK PROVIDER PROFILE        
			GameObject buttonCheckProviderProfile = null;
			if (m_container.Find("Button_CheckProviderProfile") != null) buttonCheckProviderProfile = m_container.Find("Button_CheckProviderProfile").gameObject;
			if (buttonCheckProviderProfile != null)
			{
				buttonCheckProviderProfile.gameObject.SetActive(true);
				buttonCheckProviderProfile.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.proposal.check.proovider.profile");
				buttonCheckProviderProfile.GetComponent<Button>().onClick.AddListener(OnCheckProviderProfile);
				if ((GameObject.FindObjectOfType<ScreenProviderProfileView>() != null) || (m_proposalData.User == UsersController.Instance.CurrentUser.Id))
				{
					buttonCheckProviderProfile.gameObject.SetActive(false);
				}
			}

			m_offerExclusiveContent = m_container.Find("OfferExclusive");
			if (m_offerExclusiveContent != null)
			{
				// ACCEPTING BUTTONS
				m_offerExclusiveContent.Find("PriceLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.proposal.offer.price");
				if (!m_isDisplayInfo)
				{
					m_offerExclusiveContent.Find("PriceValue").GetComponent<InputField>().text = m_proposalData.Price.ToString();
					m_offerExclusiveContent.Find("PriceValue").GetComponent<InputField>().onEndEdit.AddListener(OnProposalPrice);
				}
				else
				{
					m_offerExclusiveContent.Find("PriceValue").GetComponent<Text>().text = m_proposalData.Price.ToString();
				}
				m_offerExclusiveContent.Find("PriceCurrency").GetComponent<Text>().text = RequestsController.Instance.LastRequestConsulted.Currency.ToString();

				m_offerExclusiveContent.Find("DeadlineLabel").GetComponent<Text>().text = LanguageController.Instance.GetText("screen.create.request.description.dress");
				m_offerExclusiveContent.Find("Button_DeadlineCalendar/Title").GetComponent<Text>().text = DateConverter.TimeStampToDateTimeString(m_proposalData.Deadline);
				m_offerExclusiveContent.Find("Button_DeadlineCalendar").GetComponent<Button>().onClick.AddListener(OnCalendarClick);

				if (m_offerExclusiveContent.Find("Button_AcceptKeepLooking") != null) m_buttonAcceptAndKeepLooking = m_offerExclusiveContent.Find("Button_AcceptKeepLooking").gameObject;
				if (m_offerExclusiveContent.Find("Button_AcceptCloseDeal") != null) m_buttonCloseDeal = m_offerExclusiveContent.Find("Button_AcceptCloseDeal").gameObject;

				if (m_buttonAcceptAndKeepLooking != null) m_buttonAcceptAndKeepLooking.SetActive(false);
				if (m_buttonCloseDeal != null) m_buttonCloseDeal.SetActive(false);

				if (m_isDisplayInfo)
				{
					bool enableButtonsAccept = false;
					if (m_proposalData.User != UsersController.Instance.CurrentUser.Id)
					{
						if (m_requestData.Customer == UsersController.Instance.CurrentUser.Id)
						{
							enableButtonsAccept = true;
						}
					}

					if (m_requestData.Provider == -1)
					{
						if (enableButtonsAccept)
						{
							if (m_proposalData.Type == ProposalModel.TYPE_OFFER)
							{
								switch (m_proposalData.Accepted)
								{
									case ProposalModel.STATE_PROPOSAL_ACCEPTED_AND_KEEP_LOOKING:
										m_buttonCloseDeal.gameObject.SetActive(true);
										break;

									case ProposalModel.STATE_PROPOSAL_ACCEPTED_AND_FIXED:
										m_buttonAcceptAndKeepLooking.gameObject.SetActive(false);
										m_buttonCloseDeal.gameObject.SetActive(false);
										break;

									default:
										m_buttonAcceptAndKeepLooking.gameObject.SetActive(true);
										m_buttonCloseDeal.gameObject.SetActive(true);
										break;
								}

								m_buttonAcceptAndKeepLooking.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.proposal.accept.and.look");
								m_buttonCloseDeal.transform.Find("Title").GetComponent<Text>().text = LanguageController.Instance.GetText("message.proposal.close.deal");

								m_buttonAcceptAndKeepLooking.GetComponent<Button>().onClick.AddListener(OnAcceptAndKeepLooking);
								m_buttonCloseDeal.GetComponent<Button>().onClick.AddListener(OnAcceptAndCloseDeal);

								if (m_proposalData.Reported.Length > 0)
								{
									m_buttonAcceptAndKeepLooking.gameObject.SetActive(false);
									m_buttonCloseDeal.gameObject.SetActive(false);
								}
							}
						}
						else
						{
							m_buttonAcceptAndKeepLooking.gameObject.SetActive(false);
							m_buttonCloseDeal.gameObject.SetActive(false);
						}
					}
					else
					{
						m_buttonAcceptAndKeepLooking.gameObject.SetActive(false);
						m_buttonCloseDeal.gameObject.SetActive(false);
					}
				}
			}

			// OUTDATED OFFER
			Transform labelOfferToRevalidate = m_container.Find("OfferToRevalidate");
			if (labelOfferToRevalidate != null)
			{
				labelOfferToRevalidate.gameObject.SetActive(false);
				if (m_proposalData.Reported.Length > 0)
				{
					if (m_buttonAcceptAndKeepLooking != null) m_buttonAcceptAndKeepLooking.SetActive(false);
					if (m_buttonCloseDeal != null) m_buttonCloseDeal.SetActive(false);
					labelOfferToRevalidate.gameObject.SetActive(true);
					labelOfferToRevalidate.GetComponent<Text>().text = LanguageController.Instance.GetText("message.proposal.you.have.reported.this.offer");
				}
				else
				{
					if (!m_proposalData.IsActiveOffer())
					{
						if (m_buttonAcceptAndKeepLooking != null) m_buttonAcceptAndKeepLooking.SetActive(false);
						if (m_buttonCloseDeal != null) m_buttonCloseDeal.SetActive(false);
						if (m_proposalData.User != UsersController.Instance.CurrentUser.Id)
						{
							labelOfferToRevalidate.gameObject.SetActive(true);
							labelOfferToRevalidate.GetComponent<Text>().text = LanguageController.Instance.GetText("screen.proposal.offer.outdated.by.edit");
						}
					}
				}
			}

			m_hasBeenInited = true;
			SelectTypeMessage(m_proposalData.Type);

			UIEventController.Instance.UIEvent += new UIEventHandler(OnBasicEvent);
		}

		// -------------------------------------------
		/* 
		 * Destroy
		 */
		public override bool Destroy()
		{
			if (base.Destroy()) return true;

			m_proposalData = null;
			UIEventController.Instance.UIEvent -= OnBasicEvent;
			GameObject.Destroy(this.gameObject);

			return false;
		}

		// -------------------------------------------
		/* 
		 * BackPressed
		 */
		private void BackPressed()
		{
			if (m_isDisplayInfo)
			{
				UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
			}
			else
			{
				if (m_hasChanged)
				{
					string warning = LanguageController.Instance.GetText("message.warning");
					string description = LanguageController.Instance.GetText("message.proposal.request.exit.without.saving");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROPOSAL_EXIT_WITHOUT_SAVING);
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
				}
			}
		}

		// -------------------------------------------
		/* 
		 * SavePressed
		 */
		private void SavePressed()
		{
			if (IsReadyToPublish)
			{
				string warning = LanguageController.Instance.GetText("message.warning");
				string description = LanguageController.Instance.GetText("message.proposal.ask.a.question");
				switch (m_proposalData.Type)
				{
					case ProposalModel.TYPE_INFO:
						description = LanguageController.Instance.GetText("message.proposal.ask.a.question");
						break;

					case ProposalModel.TYPE_OFFER:
						description = LanguageController.Instance.GetText("message.proposal.create.confirmation");
						break;

					case ProposalModel.TYPE_REPORT:
						description = LanguageController.Instance.GetText("message.proposal.report.a.request");
						break;
				}
				MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROPOSAL_CONFIRMATION);
			}
		}

		// -------------------------------------------
		/* 
		 * OnTypeMessage
		 */
		private void OnTypeMessage(int _index)
		{
			if (!m_hasBeenInited) return;

			switch (_index)
			{
				case 0:
					m_proposalData.Type = ProposalModel.TYPE_OFFER;
					if (m_offerExclusiveContent != null) m_offerExclusiveContent.gameObject.SetActive(true);
					break;

				case 1:
					m_proposalData.Type = ProposalModel.TYPE_INFO;
					if (m_offerExclusiveContent != null) m_offerExclusiveContent.gameObject.SetActive(false);
					break;

				case 2:
					m_proposalData.Type = ProposalModel.TYPE_REPORT;
					if (m_offerExclusiveContent != null) m_offerExclusiveContent.gameObject.SetActive(false);
					break;
			}
			IsReadyToPublish = true;
		}

		// -------------------------------------------
		/* 
		 * SelectTypeMessage
		 */
		private void SelectTypeMessage(int _type)
		{
			switch (_type)
			{
				case ProposalModel.TYPE_OFFER:
					if (m_container.Find("TypeDropdown") != null) m_container.Find("TypeDropdown").GetComponent<Dropdown>().value = 0;
					break;

				case ProposalModel.TYPE_INFO:
					if (m_container.Find("TypeDropdown") != null) m_container.Find("TypeDropdown").GetComponent<Dropdown>().value = 1;
					break;

				case ProposalModel.TYPE_REPORT:
					if (m_container.Find("TypeDropdown") != null) m_container.Find("TypeDropdown").GetComponent<Dropdown>().value = 2;
					break;
			}
		}

		// -------------------------------------------
		/* 
		 * RemovePressed
		 */
		private void RemovePressed()
		{
			string warning = LanguageController.Instance.GetText("message.warning");
			string description = LanguageController.Instance.GetText("message.proposal.want.to.remove");
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROPOSAL_WANT_TO_REMOVE);
		}

		// -------------------------------------------
		/* 
		 * ReportPressed
		 */
		private void ReportPressed()
		{
			string warning = LanguageController.Instance.GetText("message.warning");
			string description = LanguageController.Instance.GetText("message.proposal.want.to.report.proovider");
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROPOSAL_REPORT_TOXIC_OFFER);
		}

		// -------------------------------------------
		/* 
		 * OnRequestTitle
		 */
		private void OnProposalTitle(string _data)
		{
			m_proposalData.Title = _data;
			IsReadyToPublish = true;
		}

		// -------------------------------------------
		/* 
		 * OnRequestDescription
		 */
		private void OnProposalDescription(string _data)
		{
			m_proposalData.Description = _data;
			IsReadyToPublish = true;
		}


		// -------------------------------------------
		/* 
		 * OnRequestPrice
		 */
		private void OnProposalPrice(string _data)
		{
			int priceRequest = -1;
			if (!int.TryParse(_data, out priceRequest))
			{
				priceRequest = -1;
			}
			m_proposalData.Price = priceRequest;
			IsReadyToPublish = true;
		}

		// -------------------------------------------
		/* 
		 * OnCalendarClick
		 */
		private void OnCalendarClick()
		{
			MenusScreenController.Instance.CreateNewScreen(ScreenCalendarView.SCREEN_CALENDAR, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, false, m_proposalData.Deadline.ToString());
		}


		// -------------------------------------------
		/* 
		 * MessageConfirmationServer
		 */
		private void MessageConfirmationServer()
		{
			UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
			string warning = LanguageController.Instance.GetText("message.info");
			string description = LanguageController.Instance.GetText("message.proposals.created.server.confirmation");

			switch (m_proposalData.Type)
			{
				case ProposalModel.TYPE_INFO:
					description = LanguageController.Instance.GetText("message.proposals.created.question.confirmation");
					break;

				case ProposalModel.TYPE_OFFER:
					description = LanguageController.Instance.GetText("message.proposals.created.server.confirmation");
					break;

				case ProposalModel.TYPE_REPORT:
					description = LanguageController.Instance.GetText("message.proposals.has.been.flagged.succesfully");
					break;
			}
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_OK);
		}

		// -------------------------------------------
		/* 
		 * OnAcceptAndKeepLooking
		 */
		private void OnAcceptAndKeepLooking()
		{
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			m_proposalData.Accepted = ProposalModel.STATE_PROPOSAL_ACCEPTED_AND_KEEP_LOOKING;
			UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_CALL_UPDATE_PROPOSAL, m_proposalData);
		}

		// -------------------------------------------
		/* 
		 * OnAcceptAndCloseDeal
		 */
		private void OnAcceptAndCloseDeal()
		{
			string warning = LanguageController.Instance.GetText("message.info");
			string description = LanguageController.Instance.GetText("message.proposal.confirmation.to.close.deal");
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_CONFIRMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_DEAL);
		}

		// -------------------------------------------
		/* 
		 * OnCheckProviderProfile
		 */
		private void OnCheckProviderProfile()
		{
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.loading"), null, "");
			UIEventController.Instance.DispatchUIEvent(UsersController.EVENT_USER_CALL_CONSULT_SINGLE_RECORD, m_proposalData.User);
		}

		// -------------------------------------------
		/* 
		 * OnClickReactivateOffer
		 */
		private void OnClickReactivateOffer()
		{
			MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
			UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_CALL_REACTIVATE_PROPOSAL, m_proposalData.Id);
		}

		// -------------------------------------------
		/* 
		 * OnBasicEvent
		 */
		private void OnBasicEvent(string _nameEvent, params object[] _list)
		{
			if (!this.gameObject.activeSelf) return;

			if (_nameEvent == ScreenCalendarView.EVENT_SCREENCALENDAR_SELECT_DAY)
			{
				m_proposalData.Deadline = (long)_list[0];
				m_offerExclusiveContent.Find("Button_DeadlineCalendar/Title").GetComponent<Text>().text = DateConverter.TimeStampToDateTimeString(m_proposalData.Deadline);
				IsReadyToPublish = true;
			}
			if (_nameEvent == ScreenController.EVENT_CONFIRMATION_POPUP)
			{
				string subEvent = (string)_list[2];
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_CONFIRMATION)
				{
					if ((bool)_list[1])
					{
						RequestsController.Instance.MustReloadRequests = true;
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
						UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_CALL_INSERT_NEW_PROPOSAL, m_proposalData);
					}
					else
					{
						string warning = LanguageController.Instance.GetText("message.warning");
						string description = LanguageController.Instance.GetText("message.operation.canceled");
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, "");
					}
				}
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_EXIT_WITHOUT_SAVING)
				{
					if ((bool)_list[1])
					{
						UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
					}
				}
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_OK)
				{
					UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
				}
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_KO)
				{
					UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
				}
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_WANT_TO_REMOVE)
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					if ((bool)_list[1])
					{
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
						UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_CALL_DELETE_PROPOSAL, m_proposalData.Id);
					}
				}
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_REPORT_TOXIC_OFFER)
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					if ((bool)_list[1])
					{
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
						UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_CALL_REPORT_PROPOSAL, m_proposalData.Id, UsersController.Instance.CurrentUser.Id, m_requestData.Id);
					}
				}
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_DEAL)
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					if ((bool)_list[1])
					{
						m_proposalData.Accepted = ProposalModel.STATE_PROPOSAL_ACCEPTED_AND_FIXED;
						RequestsController.Instance.MustReloadRequests = true;
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_WAIT, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("message.please.wait"), null, "");
						UIEventController.Instance.DispatchUIEvent(ProposalsController.EVENT_PROPOSAL_CALL_UPDATE_PROPOSAL, m_proposalData);
					}
				}
				if (subEvent == SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT)
				{
					UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
				}
			}
			if (_nameEvent == RequestsController.EVENT_REQUEST_RESULT_DELETE_RECORDS)
			{
				UIEventController.Instance.DispatchUIEvent(UIEventController.EVENT_SCREENMANAGER_DESTROY_SCREEN, this.gameObject);
				if ((bool)_list[0])
				{
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.info"), LanguageController.Instance.GetText("screen.proposal.request.delete.success"), null, "");
				}
				else
				{
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, LanguageController.Instance.GetText("message.error"), LanguageController.Instance.GetText("screen.proposal.request.delete.failure"), null, "");
				}
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_INSERTED_PROPOSAL)
			{
				if ((bool)_list[0])
				{
					long proposalType = (int)_list[2];
					long requestID = (long)_list[3];
					string reportedByUsersID = (string)_list[4];

					if (proposalType == ProposalModel.TYPE_REPORT)
					{
						if (m_requestData.Id == requestID)
						{
							m_requestData.Reported = reportedByUsersID;
						}
					}

					UsersController.Instance.CurrentUser.Additionaloffer = 0;
					MessageConfirmationServer();
				}
				else
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					string warning = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.proposal.insert.server.error");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, warning, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CREATION_KO);
				}
			}
			if (_nameEvent == UsersController.EVENT_USER_RESULT_FORMATTED_SINGLE_RECORD)
			{
				UserModel sUser = (UserModel)_list[0];
				if (sUser != null)
				{
					UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
					MenusScreenController.Instance.CreateNewScreen(ScreenProviderProfileView.SCREEN_PROVIDER_PROFILE_DISPLAY, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, true, sUser);
				}
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_UPDATE_PROPOSAL)
			{
				if (m_proposalData.Accepted == ProposalModel.STATE_PROPOSAL_ACCEPTED_AND_FIXED)
				{
					if ((bool)_list[0])
					{
						UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
						string info = LanguageController.Instance.GetText("message.info");
						string description = LanguageController.Instance.GetText("message.proposal.confirmation.email");
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT);
					}
				}
				else
				{
					if ((bool)_list[0])
					{
						UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
						string info = LanguageController.Instance.GetText("message.info");
						string description = LanguageController.Instance.GetText("message.proposal.confirmation.update.temporal");
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT);
					}
				}
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_DELETE_PROPOSAL)
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				if ((bool)_list[0])
				{
					string info = LanguageController.Instance.GetText("message.info");
					string description = LanguageController.Instance.GetText("message.proposal.confirmation.removed.succesfully");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT);
				}
				else
				{
					string info = LanguageController.Instance.GetText("message.erro");
					string description = LanguageController.Instance.GetText("message.proposal.confirmation.removed.failure");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT);
				}
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_REACTIVATE_PROPOSAL)
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				bool reactivated = (bool)_list[0];
				if (reactivated)
				{
					long proposalID = (long)_list[1];
					if (m_proposalData.Id == proposalID)
					{
						string info = LanguageController.Instance.GetText("message.info");
						string description = LanguageController.Instance.GetText("message.proposal.successfully.reactivated");
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT);
					}
				}
				else
				{
					string info = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.proposal.failure.reactivated");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, SUB_EVENT_SCREENPROPOSAL_RECONFIRMATION_CLOSE_EXIT);
				}
			}
			if (_nameEvent == UIEventController.EVENT_SCREENMANAGER_ANDROID_BACK_BUTTON)
			{
				BackPressed();
			}
			if (_nameEvent == ProposalsController.EVENT_PROPOSAL_RESULT_REPORT_PROPOSAL)
			{
				UIEventController.Instance.DispatchUIEvent(ScreenController.EVENT_FORCE_DESTRUCTION_POPUP);
				bool success = (bool)_list[0];
				if (success)
				{
					long proposalID = (long)_list[1];
					if (m_proposalData.Id == proposalID)
					{
						int reporterID = (int)_list[2];
						m_proposalData.Reported = reporterID.ToString();
						if (m_buttonReport != null) m_buttonReport.gameObject.SetActive(false);
						if (m_buttonAcceptAndKeepLooking != null) m_buttonAcceptAndKeepLooking.SetActive(false);
						if (m_buttonCloseDeal != null) m_buttonCloseDeal.SetActive(false);
						string info = LanguageController.Instance.GetText("message.info");
						string description = LanguageController.Instance.GetText("message.proposal.succes.reported.proovider.proposal");
						MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, "");
					}
				}
				else
				{
					string info = LanguageController.Instance.GetText("message.error");
					string description = LanguageController.Instance.GetText("message.proposal.failure.reported.proovider.proposal");
					MenusScreenController.Instance.CreateNewInformationScreen(ScreenInformationView.SCREEN_INFORMATION, UIScreenTypePreviousAction.KEEP_CURRENT_SCREEN, info, description, null, "");
				}
			}
		}
	}
}