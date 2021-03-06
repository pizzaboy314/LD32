using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hamelin;

namespace Hamelin
{
	public class IEnemy : MonoBehaviour
	{
		public TargetContainer targetContainer = new TargetContainer ();

		protected Target oldTarget;
		protected GameObject targetObject;
		protected Vector3 targetPosition;
		protected bool isFollowing = false;
		protected Vector2 offset;

		protected float timer;

		public List<Target> selfAsTargets = new List<Target>();

		protected bool readyToDie = false;

		protected void updateTarget()
		{

			Target t;
			if ((t = targetContainer.GetTarget()) != oldTarget) {
				if (oldTarget == null)
				{
					GetComponent<AudioSource>().clip = Camera.main.GetComponentInChildren<GlobalView>().getEnemyDiscoveredSound();
					GetComponent<AudioSource>().Play ();
				}
				offset = Random.insideUnitCircle / 3;
				oldTarget = t;
				if (oldTarget != null) {
					targetObject = oldTarget.getGameObject ();
					targetPosition = targetObject.transform.position;
					isFollowing = true;
				} else {
					isFollowing = false;
				}

			}
		}
		protected void updateAttack()
		{
			if (isFollowing && Vector2.Distance (targetPosition, transform.position) < 1.2 && Time.timeSinceLevelLoad - timer > getAttackTime ()) {
				timer = Time.timeSinceLevelLoad;
				if (targetObject.GetComponent<Animal>().takeDamage(getDamage ()))
				{
					isFollowing = false;
				}
			}
		}

		protected virtual float getAttackTime()
		{
			return 1;
		}
		protected virtual float getDamage()
		{
			return 5;
		}
		protected virtual float getHealth()
		{
			return 0;
		}
		protected virtual void setHealth(float newHealth)
		{
			return;
		}
		protected virtual int getPoints()
		{
			return 10;
		}
		protected virtual bool getExtraDamage(int special)
		{
			return false;
		}
		protected virtual float defenseAdjust (float damage)
		{
			return damage;
		}

		public bool takeDamage(float damage, int special)
		{
			Debug.Log ("D1:" + damage);
			if (getExtraDamage (special)) {
				damage += 5;
			}
			Debug.Log ("D2:" + damage);
			Debug.Log("H1:" + getHealth ());
			Debug.Log ("D3:" + defenseAdjust (damage));
			setHealth (getHealth () - defenseAdjust(damage));
			if (getHealth () <= 0) {
				Debug.Log ("H2:" + getHealth());
				Camera.main.GetComponentInChildren<GlobalView>().Score += getPoints();
				GameObject g = GameObject.Instantiate (Camera.main.GetComponentInChildren<GlobalView> ().killSpeaker);
				GetComponent<AudioSource> ().Stop ();
				g.GetComponent<AudioSource> ().clip = Camera.main.GetComponentInChildren<GlobalView> ().getEnemyKilledSound ();
				g.GetComponent<AudioSource> ().Play ();
				foreach (Target target in selfAsTargets) {
					target.cleanReferences ();
				}
				GameObject.Destroy (gameObject);
				return true;
			} else if (!GetComponent<AudioSource> ().isPlaying) {
				GetComponent<AudioSource> ().clip = Camera.main.GetComponentInChildren<GlobalView> ().getEnemyHurtSound ();
				GetComponent<AudioSource>().Play ();
			}
			return false;
		}
	}
}

