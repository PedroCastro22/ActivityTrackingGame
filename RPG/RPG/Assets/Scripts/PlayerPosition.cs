using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mirror;
using UnityEngine;

public class PlayerPosition : NetworkBehaviour
{

	bool _isInitialized;

	ILocationProvider _locationProvider;
	ILocationProvider LocationProvider
	{
		get
		{
			if (_locationProvider == null)
			{
				_locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
			}

			return _locationProvider;
		}
	}

	Vector3 _targetPosition;

	public override void OnStartAuthority()
    {
		enabled = true;

		LocationProviderFactory.Instance.mapManager.OnInitialized += () => _isInitialized = true;
	}

	//void Start()
	//{
	//	LocationProviderFactory.Instance.mapManager.OnInitialized += () => _isInitialized = true;
	//}

	[ClientCallback]
	private void LateUpdate() => Move();

	[Client]
	private void Move()
	{
		if (_isInitialized)
		{
			var map = LocationProviderFactory.Instance.mapManager;
			transform.localPosition = map.GeoToWorldPosition(LocationProvider.CurrentLocation.LatitudeLongitude);
		}
	}
}
