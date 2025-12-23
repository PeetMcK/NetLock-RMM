// Blood Particles Animation for Login Page - Enhanced Version
class BloodParticle {
    constructor(canvas) {
        this.canvas = canvas;
        this.trail = [];
        this.maxTrailLength = 8;
        this.glowPhase = Math.random() * Math.PI * 2;
        this.rotation = Math.random() * Math.PI * 2;
        this.rotationSpeed = (Math.random() - 0.5) * 0.05;
        this.reset();
    }
    reset() {
        this.x = Math.random() * this.canvas.width;
        this.y = -20 - Math.random() * 100;
        // More size variation - small droplets to big splatters
        const sizeType = Math.random();
        if (sizeType < 0.7) {
            this.size = Math.random() * 2 + 1; // Small droplets (1-3px)
        } else if (sizeType < 0.9) {
            this.size = Math.random() * 4 + 3; // Medium drops (3-7px)
        } else {
            this.size = Math.random() * 6 + 5; // Large splatters (5-11px)
        }
        this.speedY = Math.random() * 1.5 + 0.8;
        this.speedX = (Math.random() - 0.5) * 0.8;
        this.baseOpacity = Math.random() * 0.4 + 0.5;
        this.opacity = this.baseOpacity;
        // Enhanced blood color variations
        const redVariations = [
            { r: 139, g: 0, b: 0 },
            { r: 165, g: 0, b: 0 },
            { r: 178, g: 34, b: 34 },
            { r: 200, g: 20, b: 40 },
            { r: 220, g: 20, b: 60 },
            { r: 180, g: 0, b: 20 },
            { r: 128, g: 0, b: 0 },
            { r: 150, g: 10, b: 10 },
        ];
        this.colorObj = redVariations[Math.floor(Math.random() * redVariations.length)];
        this.color = `rgba(${this.colorObj.r}, ${this.colorObj.g}, ${this.colorObj.b}, `;
        this.trail = [];
        this.willSplash = Math.random() > 0.7;
        this.hasSplashed = false;
    }
    update() {
        // Store trail position
        if (this.trail.length < this.maxTrailLength) {
            this.trail.push({ x: this.x, y: this.y, opacity: this.opacity });
        } else {
            this.trail.shift();
            this.trail.push({ x: this.x, y: this.y, opacity: this.opacity });
        }
        this.y += this.speedY;
        this.x += this.speedX;
        this.x += Math.sin(this.y * 0.015) * 0.4;
        this.rotation += this.rotationSpeed;
        // Pulsating glow
        this.glowPhase += 0.03;
        this.opacity = this.baseOpacity + Math.sin(this.glowPhase) * 0.15;
        // Splash effect
        if (this.y > this.canvas.height - 20 && this.willSplash && !this.hasSplashed) {
            this.speedY *= 0.3;
            this.opacity *= 0.7;
            this.hasSplashed = true;
        }
        if (this.y > this.canvas.height + 30) {
            this.reset();
        }
        if (this.x < -20) this.x = this.canvas.width + 20;
        if (this.x > this.canvas.width + 20) this.x = -20;
    }
    draw(ctx) {
        // Draw trail
        for (let i = 0; i < this.trail.length; i++) {
            const trailPoint = this.trail[i];
            const trailOpacity = (i / this.trail.length) * trailPoint.opacity * 0.6;
            const trailSize = this.size * (0.3 + (i / this.trail.length) * 0.7);
            ctx.fillStyle = this.color + trailOpacity + ')';
            ctx.beginPath();
            ctx.arc(trailPoint.x, trailPoint.y, trailSize, 0, Math.PI * 2);
            ctx.fill();
        }
        ctx.save();
        ctx.translate(this.x, this.y);
        ctx.rotate(this.rotation);
        const glowIntensity = 1 + Math.sin(this.glowPhase) * 0.3;
        ctx.shadowBlur = this.size * 3 * glowIntensity;
        ctx.shadowColor = this.color + (this.opacity * 0.8) + ')';
        ctx.fillStyle = this.color + this.opacity + ')';
        ctx.beginPath();
        if (this.size > 4) {
            const points = 8;
            for (let i = 0; i < points; i++) {
                const angle = (i / points) * Math.PI * 2;
                const radius = this.size * (0.8 + Math.random() * 0.4);
                const x = Math.cos(angle) * radius;
                const y = Math.sin(angle) * radius;
                if (i === 0) {
                    ctx.moveTo(x, y);
                } else {
                    ctx.lineTo(x, y);
                }
            }
            ctx.closePath();
        } else {
            ctx.arc(0, 0, this.size, 0, Math.PI * 2);
        }
        ctx.fill();
        // Inner highlight
        ctx.shadowBlur = 0;
        ctx.fillStyle = `rgba(${this.colorObj.r + 30}, ${this.colorObj.g + 10}, ${this.colorObj.b + 10}, ${this.opacity * 0.4})`;
        ctx.beginPath();
        ctx.arc(-this.size * 0.2, -this.size * 0.2, this.size * 0.3, 0, Math.PI * 2);
        ctx.fill();
        ctx.restore();
    }
}
let bloodParticlesCanvas = null;
let bloodParticlesCtx = null;
let bloodParticles = [];
let animationFrameId = null;
function initBloodParticles() {
    console.log('🩸 Initializing enhanced blood particles...');
    bloodParticlesCanvas = document.getElementById('blood-particles-canvas');
    if (!bloodParticlesCanvas) {
        console.warn('Blood particles canvas not found');
        return;
    }
    bloodParticlesCtx = bloodParticlesCanvas.getContext('2d');
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);
    const particleCount = 80;
    bloodParticles = [];
    for (let i = 0; i < particleCount; i++) {
        bloodParticles.push(new BloodParticle(bloodParticlesCanvas));
        bloodParticles[i].y = Math.random() * bloodParticlesCanvas.height - 100;
    }
    console.log(`✨ Created ${particleCount} enhanced blood particles`);
    animateBloodParticles();
}
function resizeCanvas() {
    if (!bloodParticlesCanvas) return;
    bloodParticlesCanvas.width = window.innerWidth;
    bloodParticlesCanvas.height = window.innerHeight;
}
function animateBloodParticles() {
    if (!bloodParticlesCanvas || !bloodParticlesCtx) return;
    bloodParticlesCtx.fillStyle = 'rgba(0, 0, 0, 0.02)';
    bloodParticlesCtx.fillRect(0, 0, bloodParticlesCanvas.width, bloodParticlesCanvas.height);
    bloodParticles.forEach(particle => {
        particle.update();
        particle.draw(bloodParticlesCtx);
    });
    animationFrameId = requestAnimationFrame(animateBloodParticles);
}
function stopBloodParticles() {
    if (animationFrameId) {
        cancelAnimationFrame(animationFrameId);
        animationFrameId = null;
    }
    if (bloodParticlesCtx && bloodParticlesCanvas) {
        bloodParticlesCtx.clearRect(0, 0, bloodParticlesCanvas.width, bloodParticlesCanvas.height);
    }
}
window.initBloodParticles = initBloodParticles;
window.stopBloodParticles = stopBloodParticles;
console.log('🩸 Enhanced blood particles script loaded');
