using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shapoco.Calctus.Model.Standards {
    // https://en.wikipedia.org/wiki/E_series_of_preferred_numbers
    static class ESeries {
        public static decimal[] E3 = new decimal[] {
            1.0m, 2.2m, 4.7m
        };

        public static decimal[] E6 = new decimal[] {
            1.0m, 1.5m, 2.2m, 3.3m, 4.7m, 6.8m
        };

        public static decimal[] E12 = new decimal[] {
            1.0m, 1.2m, 1.5m, 1.8m, 2.2m, 2.7m, 3.3m, 3.9m, 4.7m, 5.6m, 6.8m, 8.2m
        };

        public static decimal[] E24 = new decimal[] {
            1.0m, 1.1m, 1.2m, 1.3m, 1.5m, 1.6m, 1.8m, 2.0m, 2.2m, 2.4m, 2.7m, 3.0m,
            3.3m, 3.6m, 3.9m, 4.3m, 4.7m, 5.1m, 5.6m, 6.2m, 6.8m, 7.5m, 8.2m, 9.1m,
        };

        public static decimal[] E48 = new decimal[] {
            1.00m, 1.05m, 1.10m, 1.15m, 1.21m, 1.27m, 1.33m, 1.40m, 1.47m, 1.54m, 1.62m, 1.69m,
            1.78m, 1.87m, 1.96m, 2.05m, 2.15m, 2.26m, 2.37m, 2.49m, 2.61m, 2.74m, 2.87m, 3.01m,
            3.16m, 3.32m, 3.48m, 3.65m, 3.83m, 4.02m, 4.22m, 4.42m, 4.64m, 4.87m, 5.11m, 5.36m,
            5.62m, 5.90m, 6.19m, 6.49m, 6.81m, 7.15m, 7.50m, 7.87m, 8.25m, 8.66m, 9.09m, 9.53m,
        };

        public static decimal[] E96 = new decimal[] {
            1.00m, 1.02m, 1.05m, 1.07m, 1.10m, 1.13m, 1.15m, 1.18m, 1.21m, 1.24m, 1.27m, 1.30m,
            1.33m, 1.37m, 1.40m, 1.43m, 1.47m, 1.50m, 1.54m, 1.58m, 1.62m, 1.65m, 1.69m, 1.74m,
            1.78m, 1.82m, 1.87m, 1.91m, 1.96m, 2.00m, 2.05m, 2.10m, 2.15m, 2.21m, 2.26m, 2.32m,
            2.37m, 2.43m, 2.49m, 2.55m, 2.61m, 2.67m, 2.74m, 2.80m, 2.87m, 2.94m, 3.01m, 3.09m,
            3.16m, 3.24m, 3.32m, 3.40m, 3.48m, 3.57m, 3.65m, 3.74m, 3.83m, 3.92m, 4.02m, 4.12m,
            4.22m, 4.32m, 4.42m, 4.53m, 4.64m, 4.75m, 4.87m, 4.99m, 5.11m, 5.23m, 5.36m, 5.49m,
            5.62m, 5.76m, 5.90m, 6.04m, 6.19m, 6.34m, 6.49m, 6.65m, 6.81m, 6.98m, 7.15m, 7.32m,
            7.50m, 7.68m, 7.87m, 8.06m, 8.25m, 8.45m, 8.66m, 8.87m, 9.09m, 9.31m, 9.53m, 9.76m,
        };

        public static decimal[] E192 = new decimal[] {
            1.00m, 1.01m, 1.02m, 1.04m, 1.05m, 1.06m, 1.07m, 1.09m, 1.10m, 1.11m, 1.13m, 1.14m,
            1.15m, 1.17m, 1.18m, 1.20m, 1.21m, 1.23m, 1.24m, 1.26m, 1.27m, 1.29m, 1.30m, 1.32m,
            1.33m, 1.35m, 1.37m, 1.38m, 1.40m, 1.42m, 1.43m, 1.45m, 1.47m, 1.49m, 1.50m, 1.52m,
            1.54m, 1.56m, 1.58m, 1.60m, 1.62m, 1.64m, 1.65m, 1.67m, 1.69m, 1.72m, 1.74m, 1.76m,
            1.78m, 1.80m, 1.82m, 1.84m, 1.87m, 1.89m, 1.91m, 1.93m, 1.96m, 1.98m, 2.00m, 2.03m,
            2.05m, 2.08m, 2.10m, 2.13m, 2.15m, 2.18m, 2.21m, 2.23m, 2.26m, 2.29m, 2.32m, 2.34m,
            2.37m, 2.40m, 2.43m, 2.46m, 2.49m, 2.52m, 2.55m, 2.58m, 2.61m, 2.64m, 2.67m, 2.71m,
            2.74m, 2.77m, 2.80m, 2.84m, 2.87m, 2.91m, 2.94m, 2.98m, 3.01m, 3.05m, 3.09m, 3.12m,
            3.16m, 3.20m, 3.24m, 3.28m, 3.32m, 3.36m, 3.40m, 3.44m, 3.48m, 3.52m, 3.57m, 3.61m,
            3.65m, 3.70m, 3.74m, 3.79m, 3.83m, 3.88m, 3.92m, 3.97m, 4.02m, 4.07m, 4.12m, 4.17m,
            4.22m, 4.27m, 4.32m, 4.37m, 4.42m, 4.48m, 4.53m, 4.59m, 4.64m, 4.70m, 4.75m, 4.81m,
            4.87m, 4.93m, 4.99m, 5.05m, 5.11m, 5.17m, 5.23m, 5.30m, 5.36m, 5.42m, 5.49m, 5.56m,
            5.62m, 5.69m, 5.76m, 5.83m, 5.90m, 5.97m, 6.04m, 6.12m, 6.19m, 6.26m, 6.34m, 6.42m,
            6.49m, 6.57m, 6.65m, 6.73m, 6.81m, 6.90m, 6.98m, 7.06m, 7.15m, 7.23m, 7.32m, 7.41m,
            7.50m, 7.59m, 7.68m, 7.77m, 7.87m, 7.96m, 8.06m, 8.16m, 8.25m, 8.35m, 8.45m, 8.56m,
            8.66m, 8.76m, 8.87m, 8.98m, 9.09m, 9.20m, 9.31m, 9.42m, 9.53m, 9.65m, 9.76m, 9.88m,
        };

        public static decimal[] GetSeries(int n) {
            switch (n) {
                case 3: return E3;
                case 6: return E6;
                case 12: return E12;
                case 24: return E24;
                case 48: return E48;
                case 96: return E96;
                case 192: return E192;
                default: throw new CalctusError("Invalid E-series number.");
            }
        }
    }
}
