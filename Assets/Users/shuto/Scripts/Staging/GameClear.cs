using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class GameClear : MonoBehaviour
{
	//Animatorをanimという変数で定義する
	private Animator anim;

	[SerializeField] private Transform MagicCircle;

	// Use this for initialization
	void Start()
	{
		// アニメーターコンポーネント取得
		anim = GetComponent<Animator>();
		GetCanvas.Instance.GameClearCanvas.alpha = 0;
		GameManager.Instance.SetGameClear = this;
	}

	//アニメーション終了時の処理
	public async UniTask GameClearStart()
	{
		anim.enabled = false;
		MagicCircle.SetParent(null, true);
		MagicCircle.DOScale(8, 4f);

		await UniTask.Delay(1000);
		await GetCanvas.Instance.FadeMask[0].FadeStart(RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position),1);
		//await DOVirtual.Float(0f, 1.0f, 1.25f,
		//	(tweenValue) => { GetCanvas.Instance.GameClearCanvas.alpha = tweenValue; });
	}
}