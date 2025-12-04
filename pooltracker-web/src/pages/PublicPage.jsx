import { useEffect, useState } from "react";
import { Toaster } from "react-hot-toast";
import { 
  Users, 
  Droplets, 
  Sparkles, 
  Thermometer, 
  Wind, 
  Cloud, 
  Sun, 
  CloudRain,
  CloudDrizzle,
  HelpCircle,
  RefreshCw,
  AlertCircle
} from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "../components/ui/card";
import { Badge } from "../components/ui/badge";
import api from "../services/api";

const PublicPage = () => {
  const [poolStatus, setPoolStatus] = useState(null);
  const [weather, setWeather] = useState(null);
  const [activeWorkers, setActiveWorkers] = useState(null);
  const [cleanings, setCleanings] = useState(null);
  const [waterQuality, setWaterQuality] = useState(null);
  const [loading, setLoading] = useState(true);
  const [lastUpdate, setLastUpdate] = useState(null);
  const [error, setError] = useState(null);

  const fetchData = async () => {
    try {
      setError(null);

      const [poolRes, weatherRes, workersRes, cleaningsRes, waterRes] = await Promise.allSettled([
        api.get('/api/pool/status'),
        api.get('/api/weather/current'),
        api.get('/api/workers/active'),
        api.get('/api/cleanings/last'),
        api.get('/api/water-quality/latest'),
      ]);

      // Handle pool status
      if (poolRes.status === 'fulfilled') {
        setPoolStatus(poolRes.value.data);
      } else {
        throw new Error("Erro a obter estado da piscina.");
      }

      // Handle weather
      if (weatherRes.status === 'fulfilled') {
        setWeather(weatherRes.value.data);
      }

      // Handle active workers
      if (workersRes.status === 'fulfilled') {
        setActiveWorkers(workersRes.value.data);
      }

      // Handle cleanings
      if (cleaningsRes.status === 'fulfilled') {
        setCleanings(cleaningsRes.value.data);
      }

      // Handle water quality
      if (waterRes.status === 'fulfilled') {
        const waterData = waterRes.value.data;
        // Transform to match expected format
        if (waterData.Criancas || waterData.Adultos) {
          setWaterQuality({
            criancas: waterData.Criancas,
            adultos: waterData.Adultos
          });
        } else {
          setWaterQuality(waterData);
        }
      }

      setLastUpdate(new Date());
    } catch (err) {
      console.error(err);
      setError(err.message || "Erro ao comunicar com o servidor.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
    const interval = setInterval(fetchData, 15000);
    return () => clearInterval(interval);
  }, []);

  const formatTime = (date) => {
    if (!date) return "-";
    if (typeof date === 'string') {
      const d = new Date(date);
      return d.toLocaleTimeString("pt-PT", { hour: "2-digit", minute: "2-digit" });
    }
    return date.toLocaleTimeString("pt-PT", { hour: "2-digit", minute: "2-digit", second: "2-digit" });
  };

  const formatDateTime = (dateString) => {
    if (!dateString) return "-";
    const date = new Date(dateString);
    return date.toLocaleString("pt-PT", { 
      day: "2-digit", 
      month: "2-digit", 
      hour: "2-digit", 
      minute: "2-digit" 
    });
  };

  const getWeatherIcon = (icon) => {
    switch (icon) {
      case "sunny":
        return <Sun className="w-5 h-5 text-yellow-400" />;
      case "cloudy":
        return <Cloud className="w-5 h-5 text-gray-400" />;
      case "overcast":
        return <Cloud className="w-5 h-5 text-gray-500" />;
      case "rain":
        return <CloudRain className="w-5 h-5 text-blue-400" />;
      case "showers":
        return <CloudDrizzle className="w-5 h-5 text-blue-300" />;
      default:
        return <HelpCircle className="w-5 h-5 text-muted-foreground" />;
    }
  };

  const occupancyPercentage = poolStatus
    ? Math.round((poolStatus.currentCount / poolStatus.maxCapacity) * 100)
    : 0;

  const statusColor = poolStatus?.isOpen ? "bg-emerald-500" : "bg-rose-600";
  const statusText = poolStatus?.isOpen ? "ABERTA" : "ENCERRADA";

  const occupancyColor =
    occupancyPercentage < 60
      ? "bg-emerald-500"
      : occupancyPercentage < 90
      ? "bg-amber-500"
      : "bg-rose-600";

  return (
    <div className="min-h-screen bg-background flex flex-col items-center p-4 sm:p-6 lg:p-10">
      <Toaster 
        position="top-right"
        toastOptions={{
          className: "bg-card border border-border",
          style: {
            background: "hsl(var(--card))",
            color: "hsl(var(--card-foreground))",
            border: "1px solid hsl(var(--border))",
          },
        }}
      />
      
      <div className="w-full max-w-5xl space-y-6">
        {/* Cabeçalho */}
        <header className="flex flex-col sm:flex-row justify-between gap-3 items-start sm:items-center">
          <div>
            <h1 className="text-2xl sm:text-3xl font-semibold text-foreground tracking-tight">
              Piscina Municipal da Sobreposta
            </h1>
            <p className="text-sm text-muted-foreground mt-1">
              R. da Piscina 22, 4715-553 Sobreposta · Tel. 253 636 948
            </p>
          </div>

          <div className="flex flex-col items-end text-right gap-1">
            <div className="flex items-center gap-2 text-xs text-muted-foreground">
              <RefreshCw className="w-3 h-3" />
              <span>Última atualização: {formatTime(lastUpdate)}</span>
            </div>
            <span className="text-xs uppercase tracking-[0.2em] text-muted-foreground">
              Ocupação em tempo real
            </span>
          </div>
        </header>

        {/* Mensagem de erro */}
        {error && (
          <Card className="border-destructive/50 bg-destructive/10">
            <CardContent className="pt-6">
              <div className="flex items-center gap-2 text-destructive">
                <AlertCircle className="w-4 h-4" />
                <span className="text-sm">{error}</span>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Loading */}
        {loading && (
          <div className="flex justify-center py-10">
            <div className="flex items-center gap-2 text-muted-foreground">
              <RefreshCw className="w-4 h-4 animate-spin" />
              <span className="text-sm">A carregar dados em tempo real...</span>
            </div>
          </div>
        )}

        {/* Conteúdo principal */}
        {!loading && poolStatus && (
          <main className="grid grid-cols-1 lg:grid-cols-3 gap-5">
            {/* Informações adicionais */}
            <section className="lg:col-span-3 grid grid-cols-1 md:grid-cols-3 gap-5">
              {/* Trabalhadores Ativos */}
              <Card>
                <CardHeader className="pb-3">
                  <CardTitle className="text-sm font-medium flex items-center gap-2">
                    <Users className="w-4 h-4" />
                    Equipa Ativa
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {activeWorkers ? (
                    <div className="space-y-3">
                      <div className="flex items-center justify-between">
                        <span className="text-sm text-muted-foreground">Nadadores Salvadores</span>
                        <Badge variant="secondary" className="text-base font-semibold">
                          {activeWorkers.counts?.nadador_salvador || activeWorkers.counts?.NadadorSalvador || 0}
                        </Badge>
                      </div>
                      <div className="flex items-center justify-between">
                        <span className="text-sm text-muted-foreground">Vigilantes</span>
                        <Badge variant="secondary" className="text-base font-semibold">
                          {activeWorkers.counts?.vigilante || activeWorkers.counts?.Vigilante || 0}
                        </Badge>
                      </div>
                    </div>
                  ) : (
                    <p className="text-sm text-muted-foreground">A carregar...</p>
                  )}
                </CardContent>
              </Card>

              {/* Última Limpeza */}
              <Card>
                <CardHeader className="pb-3">
                  <CardTitle className="text-sm font-medium flex items-center gap-2">
                    <Sparkles className="w-4 h-4" />
                    Última Limpeza
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {cleanings ? (
                    <div className="space-y-3">
                      {cleanings.balnearios && (
                        <div>
                          <span className="text-xs text-muted-foreground">Balneários</span>
                          <p className="text-sm font-medium mt-1">
                            {formatDateTime(cleanings.balnearios.cleanedAt || cleanings.balnearios.CleanedAt)}
                          </p>
                        </div>
                      )}
                      {cleanings.wc && (
                        <div>
                          <span className="text-xs text-muted-foreground">WC</span>
                          <p className="text-sm font-medium mt-1">
                            {formatDateTime(cleanings.wc.cleanedAt || cleanings.wc.CleanedAt)}
                          </p>
                        </div>
                      )}
                      {!cleanings.balnearios && !cleanings.wc && (
                        <p className="text-sm text-muted-foreground">Sem registos</p>
                      )}
                    </div>
                  ) : (
                    <p className="text-sm text-muted-foreground">A carregar...</p>
                  )}
                </CardContent>
              </Card>

              {/* Qualidade da Água */}
              <Card>
                <CardHeader className="pb-3">
                  <CardTitle className="text-sm font-medium flex items-center gap-2">
                    <Droplets className="w-4 h-4" />
                    Qualidade da Água
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  {waterQuality ? (
                    <div className="space-y-3">
                      {waterQuality.criancas && (
                        <div>
                          <span className="text-xs text-muted-foreground">Piscina Crianças</span>
                          <div className="flex items-center gap-3 mt-1">
                            <div className="flex items-center gap-1">
                              <Droplets className="w-3 h-3 text-blue-400" />
                              <span className="text-sm">
                                pH: <strong>{waterQuality.criancas.phLevel?.toFixed(1) || waterQuality.criancas.PhLevel?.toFixed(1) || 'N/A'}</strong>
                              </span>
                            </div>
                            <div className="flex items-center gap-1">
                              <Thermometer className="w-3 h-3 text-orange-400" />
                              <span className="text-sm">
                                {waterQuality.criancas.temperature?.toFixed(1) || waterQuality.criancas.Temperature?.toFixed(1) || 'N/A'}°C
                              </span>
                            </div>
                          </div>
                          {waterQuality.criancas.measuredAt && (
                            <p className="text-xs text-muted-foreground mt-1">
                              {formatDateTime(waterQuality.criancas.measuredAt || waterQuality.criancas.MeasuredAt)}
                            </p>
                          )}
                        </div>
                      )}
                      {waterQuality.adultos && (
                        <div>
                          <span className="text-xs text-muted-foreground">Piscina Adultos</span>
                          <div className="flex items-center gap-3 mt-1">
                            <div className="flex items-center gap-1">
                              <Droplets className="w-3 h-3 text-blue-400" />
                              <span className="text-sm">
                                pH: <strong>{waterQuality.adultos.phLevel?.toFixed(1) || waterQuality.adultos.PhLevel?.toFixed(1) || 'N/A'}</strong>
                              </span>
                            </div>
                            <div className="flex items-center gap-1">
                              <Thermometer className="w-3 h-3 text-orange-400" />
                              <span className="text-sm">
                                {waterQuality.adultos.temperature?.toFixed(1) || waterQuality.adultos.Temperature?.toFixed(1) || 'N/A'}°C
                              </span>
                            </div>
                          </div>
                          {waterQuality.adultos.measuredAt && (
                            <p className="text-xs text-muted-foreground mt-1">
                              {formatDateTime(waterQuality.adultos.measuredAt || waterQuality.adultos.MeasuredAt)}
                            </p>
                          )}
                        </div>
                      )}
                      {!waterQuality.criancas && !waterQuality.adultos && (
                        <p className="text-sm text-muted-foreground">Sem medições</p>
                      )}
                    </div>
                  ) : (
                    <p className="text-sm text-muted-foreground">A carregar...</p>
                  )}
                </CardContent>
              </Card>
            </section>

            {/* Cartão principal – Estado e Ocupação */}
            <Card className="lg:col-span-2">
              <CardHeader>
                <div className="flex flex-col sm:flex-row justify-between gap-4 items-start sm:items-center">
                  <div>
                    <CardTitle className="text-xs uppercase tracking-[0.25em] text-muted-foreground mb-2">
                      Estado da piscina
                    </CardTitle>
                    <Badge 
                      variant={poolStatus.isOpen ? "default" : "destructive"}
                      className="text-sm"
                    >
                      <span className={`inline-flex h-2 w-2 rounded-full mr-2 ${
                        poolStatus.isOpen ? "bg-emerald-400" : "bg-rose-500"
                      }`} />
                      {statusText}
                    </Badge>
                  </div>

                  <div className="text-right">
                    <p className="text-xs text-muted-foreground">Horário de hoje</p>
                    <p className="text-sm font-medium">
                      {poolStatus.todayOpeningHours || "Encerrado"}
                    </p>
                  </div>
                </div>
              </CardHeader>
              <CardContent>
                <div className="flex flex-col sm:flex-row gap-6 items-center sm:items-end sm:justify-between">
                  <div className="flex flex-col items-center sm:items-start">
                    <p className="text-xs uppercase tracking-[0.2em] text-muted-foreground">
                      Pessoas no interior
                    </p>
                    <div className="mt-3 flex items-end gap-2">
                      <span className="text-5xl sm:text-6xl font-semibold tabular-nums">
                        {poolStatus.currentCount}
                      </span>
                      <span className="text-sm text-muted-foreground mb-2">
                        / {poolStatus.maxCapacity}
                      </span>
                    </div>
                    <p className="mt-1 text-xs text-muted-foreground">
                      {occupancyPercentage}% da capacidade
                    </p>
                  </div>

                  <div className="w-full sm:max-w-xs">
                    <div className="flex items-center justify-between text-xs text-muted-foreground mb-2">
                      <span>Lotação</span>
                      <span>{occupancyPercentage}%</span>
                    </div>
                    <div className="h-3 rounded-full bg-muted overflow-hidden">
                      <div
                        className={`h-full ${occupancyColor} transition-[width] duration-700 ease-out`}
                        style={{ width: `${Math.min(occupancyPercentage, 100)}%` }}
                      />
                    </div>
                    <p className="mt-2 text-[11px] text-muted-foreground">
                      Valores aproximados. A informação é atualizada automaticamente.
                    </p>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Cartão de Meteorologia */}
            <Card>
              <CardHeader>
                <CardTitle className="text-xs uppercase tracking-[0.25em] text-muted-foreground">
                  Meteorologia
                </CardTitle>
                <p className="text-sm text-muted-foreground mt-1">
                  Sobreposta · Braga
                </p>
              </CardHeader>
              <CardContent>
                {weather ? (
                  <>
                    <div className="mt-5 flex items-baseline gap-3">
                      <span className="text-5xl font-semibold tabular-nums">
                        {Math.round(weather.temperatureC || weather.temperature || 0)}°
                      </span>
                      <div className="flex flex-col">
                        <span className="text-xs text-muted-foreground">
                          Sensação aproximada
                        </span>
                        <span className="text-sm">
                          {weather.description || "N/A"}
                        </span>
                      </div>
                    </div>

                    <div className="mt-4 grid grid-cols-2 gap-3">
                      <div className="rounded-lg border bg-muted/50 p-3">
                        <div className="flex items-center gap-2 text-xs text-muted-foreground mb-1">
                          <Wind className="w-3 h-3" />
                          <span>Vento</span>
                        </div>
                        <p className="text-sm font-medium">
                          {(weather.windSpeedKmh || weather.windSpeed || 0).toFixed(1)} km/h
                        </p>
                      </div>
                      <div className="rounded-lg border bg-muted/50 p-3">
                        <div className="flex items-center gap-2 text-xs text-muted-foreground mb-1">
                          {getWeatherIcon(weather.icon)}
                          <span>Condição</span>
                        </div>
                        <p className="text-sm font-medium capitalize">
                          {weather.icon === "unknown" ? "Desconhecida" : (weather.description || "N/A")}
                        </p>
                      </div>
                    </div>
                  </>
                ) : (
                  <p className="mt-4 text-sm text-muted-foreground">
                    Não foi possível obter dados de meteorologia neste momento.
                  </p>
                )}

                <p className="mt-6 text-[11px] text-muted-foreground">
                  Dados meteorológicos fornecidos por Open-Meteo. A informação
                  é meramente indicativa e pode não refletir condições exatas
                  no local.
                </p>
              </CardContent>
            </Card>
          </main>
        )}
      </div>

      {/* Footer com créditos */}
      <footer className="border-t border-border bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/60 mt-8 w-full">
        <div className="container mx-auto px-4 py-4">
          <div className="flex flex-col sm:flex-row items-center justify-between gap-2 text-xs text-muted-foreground">
            <p>© {new Date().getFullYear()} Piscina Municipal da Sobreposta</p>
            <p>
              Desenvolvido por{" "}
              <a
                href="https://github.com/ricardoguimaraes2021"
                target="_blank"
                rel="noopener noreferrer"
                className="text-primary hover:underline"
              >
                @ricardoguimaraes2021
              </a>
            </p>
          </div>
        </div>
      </footer>
    </div>
  );
};

export default PublicPage;
