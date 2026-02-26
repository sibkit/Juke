
    window.JukeApp = {
        socket: null,
        channels: {},
        connect: function() {
            const protocol = window.location.protocol === 'https:' ? 'wss://' : 'ws://';
            this.socket = new WebSocket(protocol + window.location.host + '/ws/app');
            this.socket.onmessage = (event) => {
                const msg = JSON.parse(event.data);
                if (this.channels[msg.c]) this.channels[msg.c].forEach(cb => cb(msg.p));

                document.querySelectorAll(`[data-ws-channel="${msg.c}"]`).forEach(el => {
                    const propName = el.getAttribute('data-ws-prop');
                    if (propName && msg.p[propName] !== undefined) {
                        el.innerText = msg.p[propName];
                        const flashClass = el.getAttribute('data-ws-flash');
                        if (flashClass) {
                            el.classList.remove(flashClass);
                            void el.offsetWidth;
                            el.classList.add(flashClass);
                        }
                    }
                });
            };
            this.socket.onclose = () => setTimeout(() => this.connect(), 3000);
        },
        subscribe: function(c, cb) { if(!this.channels[c]) this.channels[c] = []; this.channels[c].push(cb); }
    };
    document.addEventListener('DOMContentLoaded', () => window.JukeApp.connect());
