/** @type {import('next').NextConfig} */
const nextConfig = {
    output: 'export',
    async rewrites() {
        return [
            {
                source: '/api/:path*',
                destination: 'http://localhost/api/:path*' // Proxy to Backend
            },
            {
                source: '/images/:path*',
                destination: 'http://localhost/images/:path*' // Proxy to Backend
            }
        ]
    }
};
export default nextConfig;
