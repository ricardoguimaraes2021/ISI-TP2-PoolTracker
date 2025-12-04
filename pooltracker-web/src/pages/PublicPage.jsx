import { useState, useEffect } from 'react';
import { Users, Thermometer, Droplets, Calendar } from 'lucide-react';
import api from '../services/api';

const PublicPage = () => {
  const [poolStatus, setPoolStatus] = useState(null);
  const [weather, setWeather] = useState(null);
  const [waterQuality, setWaterQuality] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchData();
    const interval = setInterval(fetchData, 30000); // Atualizar a cada 30 segundos
    return () => clearInterval(interval);
  }, []);

  const fetchData = async () => {
    try {
      const [statusRes, weatherRes, qualityRes] = await Promise.all([
        api.get('/api/pool/status'),
        api.get('/api/weather/current'),
        api.get('/api/water-quality/latest?poolType=Criancas'),
      ]);

      setPoolStatus(statusRes.data);
      setWeather(weatherRes.data);
      setWaterQuality(qualityRes.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching data:', error);
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-xl">A carregar...</div>
      </div>
    );
  }

  const occupancyPercentage = poolStatus
    ? Math.round((poolStatus.currentCount / poolStatus.maxCapacity) * 100)
    : 0;

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-cyan-50">
      <div className="container mx-auto px-4 py-8">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-gray-800 mb-2">
            {poolStatus?.locationName || 'Piscina Municipal'}
          </h1>
          <p className="text-gray-600">{poolStatus?.address}</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {/* Estado da Piscina */}
          <div className="bg-white rounded-lg shadow-lg p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-semibold text-gray-800">Estado Atual</h2>
              <div
                className={`w-4 h-4 rounded-full ${
                  poolStatus?.isOpen ? 'bg-green-500' : 'bg-red-500'
                }`}
              />
            </div>
            <div className="space-y-4">
              <div className="flex items-center gap-3">
                <Users className="w-6 h-6 text-blue-600" />
                <div>
                  <p className="text-3xl font-bold text-gray-800">
                    {poolStatus?.currentCount || 0} / {poolStatus?.maxCapacity || 120}
                  </p>
                  <p className="text-sm text-gray-600">Pessoas na piscina</p>
                </div>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-3">
                <div
                  className="bg-blue-600 h-3 rounded-full transition-all"
                  style={{ width: `${occupancyPercentage}%` }}
                />
              </div>
              <p className="text-sm text-gray-600">
                {poolStatus?.todayOpeningHours || 'Horário não disponível'}
              </p>
            </div>
          </div>

          {/* Meteorologia */}
          {weather && (
            <div className="bg-white rounded-lg shadow-lg p-6">
              <h2 className="text-xl font-semibold text-gray-800 mb-4">Meteorologia</h2>
              <div className="space-y-3">
                <div className="flex items-center gap-3">
                  <Thermometer className="w-6 h-6 text-orange-600" />
                  <div>
                    <p className="text-2xl font-bold text-gray-800">
                      {weather.temperature}°C
                    </p>
                    <p className="text-sm text-gray-600">Temperatura</p>
                  </div>
                </div>
                <div>
                  <p className="text-lg text-gray-700">{weather.condition}</p>
                  <p className="text-sm text-gray-600">Vento: {weather.windSpeed} km/h</p>
                </div>
              </div>
            </div>
          )}

          {/* Qualidade da Água */}
          {waterQuality && (
            <div className="bg-white rounded-lg shadow-lg p-6">
              <h2 className="text-xl font-semibold text-gray-800 mb-4">Qualidade da Água</h2>
              <div className="space-y-3">
                <div className="flex items-center gap-3">
                  <Droplets className="w-6 h-6 text-cyan-600" />
                  <div>
                    <p className="text-2xl font-bold text-gray-800">
                      pH {waterQuality.phLevel}
                    </p>
                    <p className="text-sm text-gray-600">Nível de pH</p>
                  </div>
                </div>
                <div>
                  <p className="text-lg text-gray-700">
                    Temperatura: {waterQuality.temperature}°C
                  </p>
                  <p className="text-sm text-gray-500">
                    {new Date(waterQuality.measuredAt).toLocaleString('pt-PT')}
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default PublicPage;

