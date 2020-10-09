package com.example.gameproject;

import androidx.annotation.NonNull;
import androidx.core.app.ActivityCompat;
import androidx.fragment.app.FragmentActivity;

import android.Manifest;
import android.content.pm.PackageManager;
import android.location.Address;
import android.location.Geocoder;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;

import com.google.android.gms.maps.CameraUpdate;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;

import java.io.IOException;
import java.util.List;

public class MapsActivity extends FragmentActivity implements OnMapReadyCallback {

    private GoogleMap mMap;

    private LocationListener locationListener;
    private LocationManager locationManager;

    private final long MIN_TIME = 1000; // 1 second
    private final long MIN_DIST = 5; // 5 metres

    private LatLng latLng;

    @Override
    protected void onCreate(final Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_maps);
        // Obtain the SupportMapFragment and get notified when the map is ready to be used.
        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);

        locationListener = new LocationListener() {
            @Override
            public void onLocationChanged(@NonNull Location location) {
                try {
                    double lat = location.getLatitude();
                    double lng = location.getLongitude();
                    latLng = new LatLng(lat, lng);
                    Geocoder geocoder = new Geocoder(getApplicationContext());
                    try {
                        List<Address> addressList = geocoder.getFromLocation(lat, lng, 1);
                        String str = addressList.get(0).getSubLocality()+",";
                        str += addressList.get(0).getCountryName();
                        mMap.addMarker(new MarkerOptions().position(latLng).title("Pedro is here :)"));
                        mMap.moveCamera(CameraUpdateFactory.newLatLngZoom(latLng, 17.3f));
                    }
                    catch (IOException e){
                        e.printStackTrace();
                    }
                }
                catch (SecurityException e){
                    e.printStackTrace();
                }
            }
            @Override
            public void onStatusChanged(String provider, int status, Bundle extras){

            }

        };

        locationManager = (LocationManager) getSystemService(LOCATION_SERVICE);
        if(locationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER)){
            try {
                locationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, MIN_TIME, MIN_DIST, locationListener);
            }
            catch (SecurityException e){
                e.printStackTrace();
            }
        }
        else if(locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER)){
            try {
                locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, MIN_TIME, MIN_DIST, locationListener);
            }
            catch (SecurityException e){
                e.printStackTrace();
            }
        }

        ActivityCompat.requestPermissions(this, new String[] {Manifest.permission.ACCESS_FINE_LOCATION}, PackageManager.PERMISSION_GRANTED);
        ActivityCompat.requestPermissions(this, new String[] {Manifest.permission.ACCESS_COARSE_LOCATION}, PackageManager.PERMISSION_GRANTED);
    }

    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
    @Override
    public void onMapReady(GoogleMap googleMap) {
        mMap = googleMap;

        // Add a marker in Sydney and move the camera
//        LatLng sydney = new LatLng(-34, 151);
//        mMap.addMarker(new MarkerOptions().position(sydney).title("Pedro"));
//        mMap.moveCamera(CameraUpdateFactory.newLatLng(sydney));

//        locationListener = new LocationListener() {
//            @Override
//            public void onLocationChanged(@NonNull Location location) {
//                try {
//                    latLng = new LatLng(location.getLatitude(), location.getLongitude());
//                    mMap.addMarker(new MarkerOptions().position(latLng).title("My Location"));
//                    mMap.moveCamera(CameraUpdateFactory.newLatLng(latLng));
//                }
//                catch (SecurityException e){
//                    e.printStackTrace();
//                }
//            }
//        };
//
//        locationManager = (LocationManager) getSystemService(LOCATION_SERVICE);
//
//        try {
//            locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, MIN_TIME, MIN_DIST, locationListener);
//        }
//        catch (SecurityException e){
//            e.printStackTrace();
//        }
    }
}