﻿using UnityEngine;

public class SessionMNotificationBadge : MonoBehaviour 
{
	public Renderer[] badgeRenderers;
	public TextMesh badgeText;

	private bool badgeVisible = true;

	void Update ()
	{
		UpdateBadge();
	}

	private void UpdateBadge()
	{
		int badgeCount = SessionM.GetInstance().GetUnclaimedAchievementCount();

		if(badgeVisible == false && badgeCount > 0) {
			ShowBadge();
		}

		if(badgeVisible == true && badgeCount == 0) {
			HideBadge();
		}

		if(badgeCount > 99) {
			badgeCount = 99;
		}

		badgeText.text = badgeCount.ToString();
	}

	private void ShowBadge()
	{
		badgeVisible = true;

		foreach(Renderer badgeRenderer in badgeRenderers) {
			badgeRenderer.enabled = true;
		}
	}

	private void HideBadge()
	{
		badgeVisible = false;

		foreach(Renderer badgeRenderer in badgeRenderers) {
			badgeRenderer.enabled = false;
		}
	}
}